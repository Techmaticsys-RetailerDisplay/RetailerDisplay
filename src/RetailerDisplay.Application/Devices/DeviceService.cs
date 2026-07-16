using Microsoft.EntityFrameworkCore;
using RetailerDisplay.Application.Common;
using RetailerDisplay.Application.Common.Security;
using RetailerDisplay.Domain.Entities;

namespace RetailerDisplay.Application.Devices;

public class DeviceService : IDeviceService
{
    private readonly IApplicationDbContext _db;
    private readonly IDeviceKeyGenerator _keyGenerator;

    public DeviceService(IApplicationDbContext db, IDeviceKeyGenerator keyGenerator)
    {
        _db = db;
        _keyGenerator = keyGenerator;
    }

    public async Task<IReadOnlyList<DeviceDto>> ListAsync(long retailerId, CancellationToken ct = default)
    {
        var devices = await _db.Devices
            .Where(d => d.RetailerId == retailerId)
            .Include(d => d.Store)
            .Include(d => d.Playlist)
            .OrderBy(d => d.DeviceName)
            .ToListAsync(ct);
        return devices.Select(Map).ToList();
    }

    public async Task<DeviceDto> GetAsync(long retailerId, long deviceId, CancellationToken ct = default)
        => Map(await Find(retailerId, deviceId, ct, includeRefs: true));

    public async Task<DeviceDto> RegisterAsync(long retailerId, RegisterDeviceRequest r, CancellationToken ct = default)
    {
        if (r.StoreId.HasValue)
            await EnsureStore(retailerId, r.StoreId.Value, ct);
        if (r.PlaylistId.HasValue)
            await EnsurePlaylist(retailerId, r.PlaylistId.Value, ct);

        var key = await GenerateUniqueKey(ct);
        var now = DateTime.UtcNow;
        var device = new Device
        {
            RetailerId = retailerId,
            StoreId = r.StoreId,
            PlaylistId = r.PlaylistId,
            DeviceKey = key,
            DeviceName = r.DeviceName?.Trim(),
            RefreshRequested = false,
            IsRevoked = false,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };
        _db.Devices.Add(device);
        await _db.SaveChangesAsync(ct);
        return await GetAsync(retailerId, device.DeviceId, ct);
    }

    public async Task<DeviceDto> UpdateAsync(long retailerId, long deviceId, UpdateDeviceRequest r, CancellationToken ct = default)
    {
        var device = await Find(retailerId, deviceId, ct);
        if (r.StoreId.HasValue)
            await EnsureStore(retailerId, r.StoreId.Value, ct);

        device.DeviceName = r.DeviceName?.Trim();
        device.StoreId = r.StoreId;
        device.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return await GetAsync(retailerId, deviceId, ct);
    }

    public async Task<DeviceDto> AssignPlaylistAsync(long retailerId, long deviceId, AssignPlaylistRequest r, CancellationToken ct = default)
    {
        var device = await Find(retailerId, deviceId, ct);
        if (r.PlaylistId.HasValue)
            await EnsurePlaylist(retailerId, r.PlaylistId.Value, ct);

        device.PlaylistId = r.PlaylistId;
        device.RefreshRequested = true; // pick up the new assignment on next poll
        device.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return await GetAsync(retailerId, deviceId, ct);
    }

    public async Task RequestRefreshAsync(long retailerId, long deviceId, CancellationToken ct = default)
    {
        var device = await Find(retailerId, deviceId, ct);
        device.RefreshRequested = true;
        device.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task RevokeAsync(long retailerId, long deviceId, CancellationToken ct = default)
    {
        var device = await Find(retailerId, deviceId, ct);
        device.IsRevoked = true;
        device.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<DeviceStatusLogDto>> GetStatusLogAsync(long retailerId, long deviceId, CancellationToken ct = default)
    {
        await Find(retailerId, deviceId, ct);
        return await _db.DeviceStatusLogs
            .Where(l => l.DeviceId == deviceId)
            .OrderByDescending(l => l.ChangedAt)
            .Take(200)
            .Select(l => new DeviceStatusLogDto(l.Status.ToString(), l.ChangedAt))
            .ToListAsync(ct);
    }

    public async Task DeleteAsync(long retailerId, long deviceId, CancellationToken ct = default)
    {
        var device = await Find(retailerId, deviceId, ct);
        _db.Devices.Remove(device);
        await _db.SaveChangesAsync(ct);
    }

    private async Task<string> GenerateUniqueKey(CancellationToken ct)
    {
        for (var attempt = 0; attempt < 10; attempt++)
        {
            var key = _keyGenerator.Generate();
            if (!await _db.Devices.AnyAsync(d => d.DeviceKey == key, ct))
                return key;
        }
        throw new AppException("Could not generate a unique device key. Try again.", 500);
    }

    private async Task<Device> Find(long retailerId, long deviceId, CancellationToken ct, bool includeRefs = false)
    {
        var q = _db.Devices.Where(d => d.RetailerId == retailerId && d.DeviceId == deviceId);
        if (includeRefs) q = q.Include(d => d.Store).Include(d => d.Playlist);
        return await q.FirstOrDefaultAsync(ct) ?? throw AppException.NotFound("Device");
    }

    private async Task EnsureStore(long retailerId, long storeId, CancellationToken ct)
    {
        if (!await _db.Stores.AnyAsync(s => s.RetailerId == retailerId && s.StoreId == storeId, ct))
            throw AppException.NotFound("Store");
    }

    private async Task EnsurePlaylist(long retailerId, long playlistId, CancellationToken ct)
    {
        if (!await _db.Playlists.AnyAsync(p => p.RetailerId == retailerId && p.PlaylistId == playlistId, ct))
            throw AppException.NotFound("Playlist");
    }

    private static DeviceDto Map(Device d) => new(
        d.DeviceId, d.DeviceKey, d.DeviceName, d.StoreId, d.Store?.StoreName,
        d.PlaylistId, d.Playlist?.Name, DeviceStatusRules.StatusText(d.LastSeenAt), d.LastSeenAt,
        d.AppVersion, d.Orientation?.ToString(), d.ScreenWidth, d.ScreenHeight, d.IsRevoked, d.IsActive);
}
