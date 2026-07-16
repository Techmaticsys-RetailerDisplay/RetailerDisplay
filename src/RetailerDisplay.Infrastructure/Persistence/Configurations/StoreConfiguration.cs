using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailerDisplay.Domain.Entities;

namespace RetailerDisplay.Infrastructure.Persistence.Configurations;

public class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> b)
    {
        b.ToTable("tblStore");
        b.HasKey(x => x.StoreId);
        b.Property(x => x.StoreName).HasMaxLength(150).IsRequired();
        b.Property(x => x.StoreCode).HasMaxLength(50);
        b.Property(x => x.AddressLine1).HasMaxLength(200);
        b.Property(x => x.AddressLine2).HasMaxLength(200);
        b.Property(x => x.City).HasMaxLength(100);
        b.Property(x => x.State).HasMaxLength(100);
        b.Property(x => x.PostalCode).HasMaxLength(20);
        b.Property(x => x.Country).HasMaxLength(100);
        b.Property(x => x.Latitude).HasPrecision(9, 6);
        b.Property(x => x.Longitude).HasPrecision(9, 6);
        b.Property(x => x.Phone).HasMaxLength(30);
        b.Property(x => x.Email).HasMaxLength(150);
        b.Property(x => x.TimeZone).HasMaxLength(50).IsRequired();
        b.Property(x => x.IsActive).HasDefaultValue(true);

        // Unique store code per retailer (Postgres treats NULLs as distinct).
        b.HasIndex(x => new { x.RetailerId, x.StoreCode }).IsUnique();

        b.HasOne(x => x.Retailer)
            .WithMany(r => r.Stores)
            .HasForeignKey(x => x.RetailerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
