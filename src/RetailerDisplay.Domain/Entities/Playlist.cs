namespace RetailerDisplay.Domain.Entities;

/// <summary>A named, ordered collection of content assigned to devices.</summary>
public class Playlist
{
    public long PlaylistId { get; set; }
    public long RetailerId { get; set; }
    public string Name { get; set; } = null!;
    /// <summary>The single sync signal — bumps on any change to the playlist or its items.</summary>
    public int Version { get; set; } = 1;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Retailer Retailer { get; set; } = null!;
    public ICollection<PlaylistItem> Items { get; set; } = new List<PlaylistItem>();
    public ICollection<Device> Devices { get; set; } = new List<Device>();
}
