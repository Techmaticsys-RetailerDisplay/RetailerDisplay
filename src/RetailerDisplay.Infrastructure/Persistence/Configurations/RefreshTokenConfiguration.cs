using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailerDisplay.Domain.Entities;

namespace RetailerDisplay.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> b)
    {
        b.ToTable("tblRefreshToken");
        b.HasKey(x => x.TokenId);
        b.Property(x => x.TokenHash).HasMaxLength(256).IsRequired();
        b.HasIndex(x => x.TokenHash).IsUnique();
        b.Ignore(x => x.IsActive);

        b.HasOne(x => x.Retailer)
            .WithMany(r => r.RefreshTokens)
            .HasForeignKey(x => x.RetailerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
