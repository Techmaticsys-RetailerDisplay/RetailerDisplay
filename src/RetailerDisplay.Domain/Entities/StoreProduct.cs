using RetailerDisplay.Domain.Enums;

namespace RetailerDisplay.Domain.Entities;

/// <summary>
/// A product in a specific store — pulled from master or created via CSV/manually —
/// with store-editable price and details.
/// </summary>
public class StoreProduct
{
    public long StoreProductId { get; set; }
    public long RetailerId { get; set; }
    public long StoreId { get; set; }
    /// <summary>Set when pulled from the master catalog; null for CSV/manual products.</summary>
    public long? MasterProductId { get; set; }
    public ProductSource Source { get; set; }

    /// <summary>Upsert key for CSV import — unique per (StoreId, Sku).</summary>
    public string Sku { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? Brand { get; set; }

    // Beverage attributes (carried from master, store-editable)
    public string? ProductType { get; set; }
    public decimal? Abv { get; set; }
    public string? ContainerType { get; set; }
    public string? Volume { get; set; }
    public int? PackSize { get; set; }
    public int? Vintage { get; set; }

    public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public string Currency { get; set; } = "USD";
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public long? ImportBatchId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Retailer Retailer { get; set; } = null!;
    public Store Store { get; set; } = null!;
    public MasterProduct? MasterProduct { get; set; }
    public ProductImport? ImportBatch { get; set; }
    public ICollection<ContentProduct> ContentProducts { get; set; } = new List<ContentProduct>();
}
