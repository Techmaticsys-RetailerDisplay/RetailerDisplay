using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailerDisplay.Domain.Entities;

namespace RetailerDisplay.Infrastructure.Persistence.Configurations;

public class PlaylistConfiguration : IEntityTypeConfiguration<Playlist>
{
    public void Configure(EntityTypeBuilder<Playlist> b)
    {
        b.ToTable("tblPlaylist");
        b.HasKey(x => x.PlaylistId);
        b.Property(x => x.Name).HasMaxLength(150).IsRequired();
        b.Property(x => x.Version).HasDefaultValue(1);
        b.Property(x => x.IsActive).HasDefaultValue(true);

        b.HasOne(x => x.Retailer)
            .WithMany(r => r.Playlists)
            .HasForeignKey(x => x.RetailerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
