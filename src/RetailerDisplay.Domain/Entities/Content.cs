using RetailerDisplay.Domain.Enums;

namespace RetailerDisplay.Domain.Entities;

/// <summary>An atomic display unit — an image, a video, or a product list.</summary>
public class Content
{
    public long ContentId { get; set; }
    public long RetailerId { get; set; }
    public ContentType ContentType { get; set; }
    public string Name { get; set; } = null!;

    /// <summary>S3 key of the original uploaded file (image/video).</summary>
    public string? MasterKey { get; set; }
    /// <summary>JSON map of image rendition keys: { "sm", "md", "lg", "thumb" }.</summary>
    public string? MediaVariants { get; set; }
    /// <summary>Video poster / preview key.</summary>
    public string? ThumbnailKey { get; set; }
    /// <summary>Video length in seconds (probed on upload).</summary>
    public int? DurationSeconds { get; set; }
    public long? FileSizeBytes { get; set; }
    /// <summary>Hash of the underlying media, for device offline-cache invalidation.</summary>
    public string? ContentHash { get; set; }
    /// <summary>Bumps on edit so downstream playlists/devices know to re-sync.</summary>
    public int Version { get; set; } = 1;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Retailer Retailer { get; set; } = null!;
    /// <summary>Populated only for ProductList content — the picked products, ordered.</summary>
    public ICollection<ContentProduct> ContentProducts { get; set; } = new List<ContentProduct>();
    public ICollection<PlaylistItem> PlaylistItems { get; set; } = new List<PlaylistItem>();
}
