using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailerDisplay.Domain.Entities;

namespace RetailerDisplay.Infrastructure.Persistence.Configurations;

public class RetailerConfiguration : IEntityTypeConfiguration<Retailer>
{
    public void Configure(EntityTypeBuilder<Retailer> b)
    {
        b.ToTable("tblRetailer");
        b.HasKey(x => x.RetailerId);
        b.Property(x => x.Email).HasMaxLength(150).IsRequired();
        b.HasIndex(x => x.Email).IsUnique();
        b.Property(x => x.PasswordHash).HasMaxLength(255).IsRequired();
        b.Property(x => x.BusinessName).HasMaxLength(150).IsRequired();
        b.Property(x => x.ContactName).HasMaxLength(120);
        b.Property(x => x.Phone).HasMaxLength(30);
        b.Property(x => x.AddressLine1).HasMaxLength(200);
        b.Property(x => x.AddressLine2).HasMaxLength(200);
        b.Property(x => x.City).HasMaxLength(100);
        b.Property(x => x.State).HasMaxLength(100);
        b.Property(x => x.PostalCode).HasMaxLength(20);
        b.Property(x => x.Country).HasMaxLength(100);
        b.Property(x => x.ProfileCompleted).HasDefaultValue(false);
        b.Property(x => x.IsActive).HasDefaultValue(true);
    }
}
