using RetailerDisplay.Domain.Enums;

namespace RetailerDisplay.Domain.Entities;

/// <summary>Ordered content within a playlist, with per-item display duration and fit.</summary>
public class PlaylistItem
{
    public long PlaylistItemId { get; set; }
    public long PlaylistId { get; set; }
    public long ContentId { get; set; }
    public int SortOrder { get; set; }
    /// <summary>Seconds to show an image / product-list item. Null &rarr; default (10s). Ignored for video.</summary>
    public int? DurationSeconds { get; set; }
    public FitMode FitMode { get; set; } = FitMode.Contain;
    public DateTime CreatedAt { get; set; }

    public Playlist Playlist { get; set; } = null!;
    public Content Content { get; set; } = null!;
}
