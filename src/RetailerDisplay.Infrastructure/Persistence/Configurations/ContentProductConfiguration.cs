using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailerDisplay.Domain.Entities;

namespace RetailerDisplay.Infrastructure.Persistence.Configurations;

public class ContentProductConfiguration : IEntityTypeConfiguration<ContentProduct>
{
    public void Configure(EntityTypeBuilder<ContentProduct> b)
    {
        b.ToTable("tblContentProduct");
        b.HasKey(x => x.Id);

        // A product appears at most once per product-list content.
        b.HasIndex(x => new { x.ContentId, x.StoreProductId }).IsUnique();

        b.HasOne(x => x.Content)
            .WithMany(c => c.ContentProducts)
            .HasForeignKey(x => x.ContentId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.StoreProduct)
            .WithMany(p => p.ContentProducts)
            .HasForeignKey(x => x.StoreProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
