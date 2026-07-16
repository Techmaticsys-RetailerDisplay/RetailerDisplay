using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RetailerDisplay.Application.Common;
using RetailerDisplay.Application.Common.Storage;
using RetailerDisplay.Application.Devices;
using RetailerDisplay.Domain.Entities;
using RetailerDisplay.Domain.Enums;

namespace RetailerDisplay.Application.DeviceApi;

public class DeviceApiService : IDeviceApiService
{
    private static readonly TimeSpan UrlExpiry = TimeSpan.FromHours(6);
    private readonly IApplicationDbContext _db;
    private readonly IMediaStorage _storage;

    public DeviceApiService(IApplicationDbContext db, IMediaStorage storage)
    {
        _db = db;
        _storage = storage;
    }

    public async Task<PairResponse> PairAsync(PairRequest request, CancellationToken ct = default)
    {
        var device = await Resolve(request.DeviceKey, ct);

        ApplyReported(device, request.ScreenWidth, request.ScreenHeight, request.Orientation, request.AppVersion);
        await MarkSeen(device, ct);

        var version = await CurrentVersion(device, ct);
        return new PairResponse(device.DeviceId, device.DeviceName, device.PlaylistId, version);
    }

    public async Task<ManifestResponse?> GetManifestAsync(string deviceKey, int? knownVersion, CancellationToken ct = default)
    {
        var device = await Resolve(deviceKey, ct);
        await MarkSeen(device, ct);

        var version = await CurrentVersion(device, ct);

        // Nothing new and no refresh pending -> let the caller reply 304.
        if (knownVersion == version && !device.RefreshRequested)
            return null;

        var items = new List<ManifestItem>();
        if (device.PlaylistId is { } playlistId)
        {
            var playlistItems = await _db.PlaylistItems
                .Where(i => i.PlaylistId == playlistId)
                .OrderBy(i => i.SortOrder)
                .Include(i => i.Content)
                .ToListAsync(ct);

            foreach (var item in playlistItems)
                items.Add(await BuildItem(item, device, ct));
        }

        // Clear the refresh flag and record the synced version.
        device.RefreshRequested = false;
        device.LastSyncedVersion = version;
        device.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        return new ManifestResponse(version, false, items);
    }

    public async Task<HeartbeatResponse> HeartbeatAsync(string deviceKey, HeartbeatRequest request, CancellationToken ct = default)
    {
        var device = await Resolve(deviceKey, ct);
        ApplyReported(device, request.ScreenWidth, request.ScreenHeight, request.Orientation, request.AppVersion);
        await MarkSeen(device, ct);

        var version = await CurrentVersion(device, ct);
        return new HeartbeatResponse(version, device.RefreshRequested);
    }

    private async Task<ManifestItem> BuildItem(PlaylistItem item, Device device, CancellationToken ct)
    {
        var content = item.Content;
        var fit = item.FitMode.ToString();

        switch (content.ContentType)
        {
            case ContentType.Image:
            {
                var key = PickRendition(content.MediaVariants, device.ScreenWidth) ?? content.MasterKey;
                var url = key is null ? null : await _storage.CreateDownloadUrlAsync(key, UrlExpiry, ct);
                var duration = item.DurationSeconds ?? MediaLimits.DefaultImageDurationSeconds;
                return new ManifestItem(content.ContentId, "Image", url, duration, fit, content.ContentHash, null);
            }
            case ContentType.Video:
            {
                var url = content.MasterKey is null ? null : await _storage.CreateDownloadUrlAsync(content.MasterKey, UrlExpiry, ct);
                var duration = content.DurationSeconds ?? MediaLimits.MaxVideoSeconds;
                return new ManifestItem(content.ContentId, "Video", url, duration, fit, content.ContentHash, null);
            }
            case ContentType.ProductList:
            {
                var products = await _db.ContentProducts
                    .Where(cp => cp.ContentId == content.ContentId)
                    .OrderBy(cp => cp.SortOrder)
                    .Select(cp => new { cp.StoreProduct.ProductName, cp.StoreProduct.Price, cp.StoreProduct.SalePrice, cp.StoreProduct.ImageUrl })
                    .ToListAsync(ct);

                var list = new List<ManifestProduct>(products.Count);
                foreach (var p in products)
                {
                    string? img = string.IsNullOrWhiteSpace(p.ImageUrl)
                        ? null
                        : await _storage.CreateDownloadUrlAsync(p.ImageUrl!, UrlExpiry, ct);
                    list.Add(new ManifestProduct(p.ProductName, p.Price, p.SalePrice, img));
                }

                var duration = item.DurationSeconds ?? MediaLimits.DefaultImageDurationSeconds;
                return new ManifestItem(content.ContentId, "ProductList", null, duration, fit, content.ContentHash, list);
            }
            default:
                return new ManifestItem(content.ContentId, "Unknown", null, MediaLimits.DefaultImageDurationSeconds, fit, content.ContentHash, null);
        }
    }

    /// <summary>Chooses a rendition key for the device's screen width (sm≤720, md≤1080, else lg).</summary>
    private static string? PickRendition(string? mediaVariantsJson, int? screenWidth)
    {
        if (string.IsNullOrWhiteSpace(mediaVariantsJson)) return null;
        Dictionary<string, string>? variants;
        try { variants = JsonSerializer.Deserialize<Dictionary<string, string>>(mediaVariantsJson); }
        catch { return null; }
        if (variants is null || variants.Count == 0) return null;

        var width = screenWidth ?? 1080;
        var preferred = width <= 720 ? "sm" : width <= 1080 ? "md" : "lg";

        return variants.GetValueOrDefault(preferred)
            ?? variants.GetValueOrDefault("md")
            ?? variants.GetValueOrDefault("lg")
            ?? variants.GetValueOrDefault("sm")
            ?? variants.Values.FirstOrDefault();
    }

    private async Task<int> CurrentVersion(Device device, CancellationToken ct)
    {
        if (device.PlaylistId is not { } playlistId) return 0;
        return await _db.Playlists.Where(p => p.PlaylistId == playlistId).Select(p => p.Version).FirstOrDefaultAsync(ct);
    }

    private static void ApplyReported(Device device, int? width, int? height, string? orientation, string? appVersion)
    {
        if (width.HasValue) device.ScreenWidth = width;
        if (height.HasValue) device.ScreenHeight = height;
        if (!string.IsNullOrWhiteSpace(appVersion)) device.AppVersion = appVersion;
        if (!string.IsNullOrWhiteSpace(orientation) && Enum.TryParse<Orientation>(orientation, true, out var o))
            device.Orientation = o;
    }

    private async Task MarkSeen(Device device, CancellationToken ct)
    {
        var wasOnline = DeviceStatusRules.IsOnline(device.LastSeenAt);
        if (!wasOnline)
        {
            _db.DeviceStatusLogs.Add(new DeviceStatusLog
            {
                DeviceId = device.DeviceId,
                Status = DeviceStatus.Online,
                ChangedAt = DateTime.UtcNow
            });
        }
        device.LastSeenAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    private async Task<Device> Resolve(string deviceKey, CancellationToken ct)
    {
        var device = await _db.Devices.FirstOrDefaultAsync(d => d.DeviceKey == deviceKey, ct);
        if (device is null || device.IsRevoked || !device.IsActive)
            throw AppException.Unauthorized("Invalid or revoked device key.");
        return device;
    }
}
