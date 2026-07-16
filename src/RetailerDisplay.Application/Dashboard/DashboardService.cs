using Microsoft.EntityFrameworkCore;
using RetailerDisplay.Application.Common;
using RetailerDisplay.Application.Devices;

namespace RetailerDisplay.Application.Dashboard;

public record DashboardSummaryDto(
    int DevicesOnline,
    int DevicesOffline,
    int TotalDevices,
    int TotalContent,
    int ActivePlaylists,
    int TotalProducts,
    DateTime? LastImportAt);

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(long retailerId, CancellationToken ct = default);
}

public class DashboardService : IDashboardService
{
    private readonly IApplicationDbContext _db;

    public DashboardService(IApplicationDbContext db) => _db = db;

    public async Task<DashboardSummaryDto> GetSummaryAsync(long retailerId, CancellationToken ct = default)
    {
        var lastSeen = await _db.Devices
            .Where(d => d.RetailerId == retailerId)
            .Select(d => d.LastSeenAt)
            .ToListAsync(ct);

        var online = lastSeen.Count(DeviceStatusRules.IsOnline);
        var total = lastSeen.Count;

        var totalContent = await _db.Contents.CountAsync(c => c.RetailerId == retailerId, ct);
        var activePlaylists = await _db.Playlists.CountAsync(p => p.RetailerId == retailerId && p.IsActive, ct);
        var totalProducts = await _db.StoreProducts.CountAsync(p => p.RetailerId == retailerId, ct);

        var lastImportAt = await _db.ProductImports
            .Where(i => i.RetailerId == retailerId)
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => (DateTime?)i.CreatedAt)
            .FirstOrDefaultAsync(ct);

        return new DashboardSummaryDto(online, total - online, total, totalContent, activePlaylists, totalProducts, lastImportAt);
    }
}
