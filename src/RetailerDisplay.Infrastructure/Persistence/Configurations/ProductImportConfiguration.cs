using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailerDisplay.Domain.Entities;

namespace RetailerDisplay.Infrastructure.Persistence.Configurations;

public class ProductImportConfiguration : IEntityTypeConfiguration<ProductImport>
{
    public void Configure(EntityTypeBuilder<ProductImport> b)
    {
        b.ToTable("tblProductImport");
        b.HasKey(x => x.ImportId);
        b.Property(x => x.FileName).HasMaxLength(255).IsRequired();
        b.Property(x => x.Status).HasConversion<short>();
        b.Property(x => x.ErrorReportUrl).HasMaxLength(500);

        b.HasOne(x => x.Retailer)
            .WithMany(r => r.ProductImports)
            .HasForeignKey(x => x.RetailerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
