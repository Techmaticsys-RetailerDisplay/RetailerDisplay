namespace RetailerDisplay.Domain.Entities;

/// <summary>A retailer account — the tenant root. One per email login.</summary>
public class Retailer
{
    public long RetailerId { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string BusinessName { get; set; } = null!;
    public string? ContactName { get; set; }
    public string? Phone { get; set; }

    // Full profile (filled in by the retailer after first login)
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    /// <summary>True once the retailer has filled in the required profile fields.</summary>
    public bool ProfileCompleted { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<Store> Stores { get; set; } = new List<Store>();
    public ICollection<StoreProduct> Products { get; set; } = new List<StoreProduct>();
    public ICollection<Content> Contents { get; set; } = new List<Content>();
    public ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
    public ICollection<Device> Devices { get; set; } = new List<Device>();
    public ICollection<ProductImport> ProductImports { get; set; } = new List<ProductImport>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
