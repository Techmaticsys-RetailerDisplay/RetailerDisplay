using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailerDisplay.Domain.Entities;

namespace RetailerDisplay.Infrastructure.Persistence.Configurations;

public class StoreProductConfiguration : IEntityTypeConfiguration<StoreProduct>
{
    public void Configure(EntityTypeBuilder<StoreProduct> b)
    {
        b.ToTable("tblStoreProduct");
        b.HasKey(x => x.StoreProductId);
        b.Property(x => x.Source).HasConversion<short>();
        b.Property(x => x.Sku).HasMaxLength(80).IsRequired();
        b.Property(x => x.ProductName).HasMaxLength(250).IsRequired();
        b.Property(x => x.Category).HasMaxLength(120);
        b.Property(x => x.Brand).HasMaxLength(120);
        b.Property(x => x.ProductType).HasMaxLength(40);
        b.Property(x => x.Abv).HasPrecision(4, 2);
        b.Property(x => x.ContainerType).HasMaxLength(40);
        b.Property(x => x.Volume).HasMaxLength(30);
        b.Property(x => x.Price).HasPrecision(10, 2);
        b.Property(x => x.SalePrice).HasPrecision(10, 2);
        b.Property(x => x.Currency).HasMaxLength(3).IsFixedLength().HasDefaultValue("USD");
        b.Property(x => x.ImageUrl).HasMaxLength(500);
        b.Property(x => x.IsActive).HasDefaultValue(true);

        // Upsert key for CSV import — unique SKU per store.
        b.HasIndex(x => new { x.StoreId, x.Sku }).IsUnique();

        b.HasOne(x => x.Retailer)
            .WithMany(r => r.Products)
            .HasForeignKey(x => x.RetailerId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Store)
            .WithMany(s => s.Products)
            .HasForeignKey(x => x.StoreId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.MasterProduct)
            .WithMany(m => m.StoreProducts)
            .HasForeignKey(x => x.MasterProductId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasOne(x => x.ImportBatch)
            .WithMany(i => i.Products)
            .HasForeignKey(x => x.ImportBatchId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
