namespace RetailerDisplay.Domain.Entities;

/// <summary>
/// Central master catalog product. Global — no RetailerId. Curated by the main Admin;
/// stores pull copies into <see cref="StoreProduct"/>.
/// </summary>
public class MasterProduct
{
    public long MasterProductId { get; set; }
    public string? Sku { get; set; }
    public string? Upc { get; set; }
    public string ProductName { get; set; } = null!;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? Brand { get; set; }

    // Beverage attributes
    public string? ProductType { get; set; }
    public decimal? Abv { get; set; }
    public string? ContainerType { get; set; }
    public string? Volume { get; set; }
    public int? PackSize { get; set; }
    public int? Vintage { get; set; }

    public string? DefaultImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<StoreProduct> StoreProducts { get; set; } = new List<StoreProduct>();
}
