using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailerDisplay.Domain.Entities;

namespace RetailerDisplay.Infrastructure.Persistence.Configurations;

public class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> b)
    {
        b.ToTable("tblDevice");
        b.HasKey(x => x.DeviceId);
        b.Property(x => x.DeviceKey).HasMaxLength(8).IsFixedLength().IsRequired();
        b.HasIndex(x => x.DeviceKey).IsUnique();
        b.Property(x => x.DeviceName).HasMaxLength(120);
        b.Property(x => x.Orientation).HasConversion<short>();
        b.Property(x => x.AppVersion).HasMaxLength(30);
        b.Property(x => x.RefreshRequested).HasDefaultValue(false);
        b.Property(x => x.IsRevoked).HasDefaultValue(false);
        b.Property(x => x.IsActive).HasDefaultValue(true);

        b.HasOne(x => x.Retailer)
            .WithMany(r => r.Devices)
            .HasForeignKey(x => x.RetailerId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Store)
            .WithMany(s => s.Devices)
            .HasForeignKey(x => x.StoreId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasOne(x => x.Playlist)
            .WithMany(p => p.Devices)
            .HasForeignKey(x => x.PlaylistId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
