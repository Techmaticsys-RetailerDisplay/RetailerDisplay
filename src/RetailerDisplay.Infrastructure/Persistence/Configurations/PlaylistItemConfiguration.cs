using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailerDisplay.Domain.Entities;

namespace RetailerDisplay.Infrastructure.Persistence.Configurations;

public class PlaylistItemConfiguration : IEntityTypeConfiguration<PlaylistItem>
{
    public void Configure(EntityTypeBuilder<PlaylistItem> b)
    {
        b.ToTable("tblPlaylistItem");
        b.HasKey(x => x.PlaylistItemId);
        b.Property(x => x.FitMode).HasConversion<short>().HasDefaultValue(Domain.Enums.FitMode.Contain);

        b.HasOne(x => x.Playlist)
            .WithMany(p => p.Items)
            .HasForeignKey(x => x.PlaylistId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Content)
            .WithMany(c => c.PlaylistItems)
            .HasForeignKey(x => x.ContentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
