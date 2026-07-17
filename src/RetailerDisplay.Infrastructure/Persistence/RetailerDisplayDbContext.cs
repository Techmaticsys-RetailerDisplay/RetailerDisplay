using Microsoft.EntityFrameworkCore;
using RetailerDisplay.Application.Common;
using RetailerDisplay.Domain.Entities;

namespace RetailerDisplay.Infrastructure.Persistence;

/// <summary>EF Core context for the RetailerDisplay PostgreSQL database.</summary>
public class RetailerDisplayDbContext : DbContext, IApplicationDbContext
{
    public RetailerDisplayDbContext(DbContextOptions<RetailerDisplayDbContext> options)
        : base(options)
    {
    }

    public DbSet<Retailer> Retailers => Set<Retailer>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<MasterProduct> MasterProducts => Set<MasterProduct>();
    public DbSet<StoreProduct> StoreProducts => Set<StoreProduct>();
    public DbSet<ProductImport> ProductImports => Set<ProductImport>();
    public DbSet<Content> Contents => Set<Content>();
    public DbSet<ContentProduct> ContentProducts => Set<ContentProduct>();
    public DbSet<Playlist> Playlists => Set<Playlist>();
    public DbSet<PlaylistItem> PlaylistItems => Set<PlaylistItem>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<DeviceStatusLog> DeviceStatusLogs => Set<DeviceStatusLog>();
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RetailerDisplayDbContext).Assembly);
    }
}
