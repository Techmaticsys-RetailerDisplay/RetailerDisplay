using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailerDisplay.Domain.Entities;

namespace RetailerDisplay.Infrastructure.Persistence.Configurations;

public class DeviceStatusLogConfiguration : IEntityTypeConfiguration<DeviceStatusLog>
{
    public void Configure(EntityTypeBuilder<DeviceStatusLog> b)
    {
        b.ToTable("tblDeviceStatusLog");
        b.HasKey(x => x.LogId);
        b.Property(x => x.Status).HasConversion<short>();

        b.HasIndex(x => new { x.DeviceId, x.ChangedAt });

        b.HasOne(x => x.Device)
            .WithMany(d => d.StatusLogs)
            .HasForeignKey(x => x.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
