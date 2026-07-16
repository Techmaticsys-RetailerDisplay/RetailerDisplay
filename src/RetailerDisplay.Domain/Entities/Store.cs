namespace RetailerDisplay.Domain.Entities;

/// <summary>A physical store / location under a retailer.</summary>
public class Store
{
    public long StoreId { get; set; }
    public long RetailerId { get; set; }
    public string StoreName { get; set; } = null!;
    public string? StoreCode { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    /// <summary>IANA time zone, e.g. America/New_York.</summary>
    public string TimeZone { get; set; } = "UTC";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Retailer Retailer { get; set; } = null!;
    public ICollection<Device> Devices { get; set; } = new List<Device>();
    public ICollection<StoreProduct> Products { get; set; } = new List<StoreProduct>();
}
