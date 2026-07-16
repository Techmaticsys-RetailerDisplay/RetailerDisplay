using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailerDisplay.Domain.Entities;

namespace RetailerDisplay.Infrastructure.Persistence.Configurations;

public class MasterProductConfiguration : IEntityTypeConfiguration<MasterProduct>
{
    public void Configure(EntityTypeBuilder<MasterProduct> b)
    {
        b.ToTable("tblMasterProduct");
        b.HasKey(x => x.MasterProductId);
        b.Property(x => x.Sku).HasMaxLength(80);
        b.Property(x => x.Upc).HasMaxLength(50);
        b.Property(x => x.ProductName).HasMaxLength(250).IsRequired();
        b.Property(x => x.Category).HasMaxLength(120);
        b.Property(x => x.Brand).HasMaxLength(120);
        b.Property(x => x.ProductType).HasMaxLength(40);
        b.Property(x => x.Abv).HasPrecision(4, 2);
        b.Property(x => x.ContainerType).HasMaxLength(40);
        b.Property(x => x.Volume).HasMaxLength(30);
        b.Property(x => x.DefaultImageUrl).HasMaxLength(500);
        b.Property(x => x.IsActive).HasDefaultValue(true);

        b.HasIndex(x => x.Sku).IsUnique();
        b.HasIndex(x => x.Upc).IsUnique();
    }
}
