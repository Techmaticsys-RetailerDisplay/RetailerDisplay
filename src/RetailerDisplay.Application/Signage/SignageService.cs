using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RetailerDisplay.Application.Common;
using RetailerDisplay.Application.Common.Storage;
using RetailerDisplay.Domain.Enums;

namespace RetailerDisplay.Application.Signage;

public class SignageService : ISignageService
{
    private static readonly TimeSpan UrlExpiry = TimeSpan.FromHours(6);
    private const int DefaultDisplaySeconds = 10;

    private readonly IApplicationDbContext _db;
    private readonly IMediaStorage _storage;

    public SignageService(IApplicationDbContext db, IMediaStorage storage)
    {
        _db = db;
        _storage = storage;
    }

    public async Task<SignageResultDto> GetSignageItemResultAsync(SignageRequest request, CancellationToken ct = default)
    {
        // The device authenticates with the AccessKey we generated (stored as Device.DeviceKey).
        // DeviceId is the device's own hardware id — reported for reference only, echoed back.
        var accessKey = request.AccessKey?.Trim() ?? "";
        var result = new SignageDto
        {
            DeviceId = request.DeviceId,
            AccessKey = request.AccessKey,
            ServerHitTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
        };

        var device = await _db.Devices.FirstOrDefaultAsync(d => d.DeviceKey == accessKey, ct);
        if (device is null || device.IsRevoked || !device.IsActive)
        {
            result.IsAccess = "False";
            result.Message = "Invalid or inactive access key.";
            return new SignageResultDto { GetSignageItemResult = result };
        }

        // Treat a fetch as a heartbeat.
        device.LastSeenAt = DateTime.UtcNow;
        device.RefreshRequested = false;
        device.UpdatedAt = DateTime.UtcNow;

        result.IsAccess = "True";
        result.IsVertical = device.Orientation == Orientation.Portrait;

        if (device.PlaylistId is { } playlistId)
        {
            var version = await _db.Playlists.Where(p => p.PlaylistId == playlistId).Select(p => p.Version).FirstOrDefaultAsync(ct);
            device.LastSyncedVersion = version;

            var items = await _db.PlaylistItems
                .Where(i => i.PlaylistId == playlistId)
                .OrderBy(i => i.SortOrder)
                .Include(i => i.Content)
                .ToListAsync(ct);

            var priority = 1;
            foreach (var item in items)
            {
                var displayTime = item.DurationSeconds ?? item.Content.DurationSeconds ?? DefaultDisplaySeconds;
                switch (item.Content.ContentType)
                {
                    case ContentType.Image:
                    {
                        var key = PickImageKey(item.Content.MediaVariants, item.Content.MasterKey, device.ScreenWidth);
                        var url = key is null ? null : await _storage.CreateDownloadUrlAsync(key, UrlExpiry, ct);
                        result.Items.Add(new SignageItemDto
                        {
                            ItemPriority = priority++,
                            ItemType = "CustomImage",
                            ItemDisplayTime = displayTime,
                            Item = new SignageImageItem { CustomImage = url, ID = item.Content.ContentId.ToString() }
                        });
                        break;
                    }
                    case ContentType.Video:
                    {
                        var url = item.Content.MasterKey is null ? null : await _storage.CreateDownloadUrlAsync(item.Content.MasterKey, UrlExpiry, ct);
                        result.Items.Add(new SignageItemDto
                        {
                            ItemPriority = priority++,
                            ItemType = "Video",
                            ItemDisplayTime = displayTime,
                            Item = new SignageVideoItem { VideoUrl = url, ID = item.Content.ContentId.ToString() }
                        });
                        break;
                    }
                    case ContentType.ProductList:
                    {
                        var products = await _db.ContentProducts
                            .Where(cp => cp.ContentId == item.Content.ContentId)
                            .OrderBy(cp => cp.SortOrder)
                            .Select(cp => cp.StoreProduct)
                            .ToListAsync(ct);

                        foreach (var p in products)
                        {
                            string? img = string.IsNullOrWhiteSpace(p.ImageUrl)
                                ? null : await _storage.CreateDownloadUrlAsync(p.ImageUrl!, UrlExpiry, ct);
                            result.Items.Add(new SignageItemDto
                            {
                                ItemPriority = priority++,
                                ItemType = "Product",
                                ItemDisplayTime = displayTime,
                                Item = new SignageProductItem
                                {
                                    ProductName = p.ProductName,
                                    ProductPrice = p.Price,
                                    ProductOfferPrice = p.SalePrice ?? p.Price,
                                    ProductImage = img,
                                    ProductCurrencySymbol = p.Currency == "USD" ? "$" : p.Currency,
                                    ProductDescription = p.Description,
                                    ID = p.StoreProductId.ToString()
                                }
                            });
                        }
                        break;
                    }
                }
            }
        }

        await _db.SaveChangesAsync(ct);
        return new SignageResultDto { GetSignageItemResult = result };
    }

    private static string? PickImageKey(string? mediaVariantsJson, string? masterKey, int? screenWidth)
    {
        if (string.IsNullOrWhiteSpace(mediaVariantsJson)) return masterKey;
        Dictionary<string, string>? variants;
        try { variants = JsonSerializer.Deserialize<Dictionary<string, string>>(mediaVariantsJson); }
        catch { return masterKey; }
        if (variants is null || variants.Count == 0) return masterKey;

        var width = screenWidth ?? 1080;
        var preferred = width <= 720 ? "sm" : width <= 1080 ? "md" : "lg";
        return variants.GetValueOrDefault(preferred)
            ?? variants.GetValueOrDefault("md") ?? variants.GetValueOrDefault("lg")
            ?? variants.GetValueOrDefault("sm") ?? masterKey;
    }
}
