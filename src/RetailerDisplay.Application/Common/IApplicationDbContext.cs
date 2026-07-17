using Microsoft.EntityFrameworkCore;
using RetailerDisplay.Domain.Entities;

namespace RetailerDisplay.Application.Common;

/// <summary>
/// Abstraction over the EF Core context so the Application layer can query and persist
/// without depending on Infrastructure. Implemented by RetailerDisplayDbContext.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Retailer> Retailers { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Store> Stores { get; }
    DbSet<MasterProduct> MasterProducts { get; }
    DbSet<StoreProduct> StoreProducts { get; }
    DbSet<ProductImport> ProductImports { get; }
    DbSet<Domain.Entities.Content> Contents { get; }
    DbSet<ContentProduct> ContentProducts { get; }
    DbSet<Playlist> Playlists { get; }
    DbSet<PlaylistItem> PlaylistItems { get; }
    DbSet<Device> Devices { get; }
    DbSet<DeviceStatusLog> DeviceStatusLogs { get; }
    DbSet<AdminUser> AdminUsers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
