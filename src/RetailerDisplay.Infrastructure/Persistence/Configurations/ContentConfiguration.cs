using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailerDisplay.Domain.Entities;

namespace RetailerDisplay.Infrastructure.Persistence.Configurations;

public class ContentConfiguration : IEntityTypeConfiguration<Content>
{
    public void Configure(EntityTypeBuilder<Content> b)
    {
        b.ToTable("tblContent");
        b.HasKey(x => x.ContentId);
        b.Property(x => x.ContentType).HasConversion<short>();
        b.Property(x => x.Name).HasMaxLength(150).IsRequired();
        b.Property(x => x.MasterKey).HasMaxLength(500);
        b.Property(x => x.MediaVariants).HasColumnType("jsonb");
        b.Property(x => x.ThumbnailKey).HasMaxLength(500);
        b.Property(x => x.ContentHash).HasMaxLength(64);
        b.Property(x => x.Version).HasDefaultValue(1);
        b.Property(x => x.IsActive).HasDefaultValue(true);

        b.HasOne(x => x.Retailer)
            .WithMany(r => r.Contents)
            .HasForeignKey(x => x.RetailerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
