namespace RetailerDisplay.Domain.Entities;

/// <summary>
/// A product picked into a ProductList content, with its display order.
/// Links a <see cref="Content"/> (type = ProductList) to a <see cref="StoreProduct"/>.
/// </summary>
public class ContentProduct
{
    public long Id { get; set; }
    public long ContentId { get; set; }
    public long StoreProductId { get; set; }
    public int SortOrder { get; set; }

    public Content Content { get; set; } = null!;
    public StoreProduct StoreProduct { get; set; } = null!;
}
