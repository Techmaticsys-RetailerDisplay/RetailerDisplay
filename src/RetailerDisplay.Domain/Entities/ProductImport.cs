using RetailerDisplay.Domain.Enums;

namespace RetailerDisplay.Domain.Entities;

/// <summary>One row per CSV upload — powers the "last import" summary and error report.</summary>
public class ProductImport
{
    public long ImportId { get; set; }
    public long RetailerId { get; set; }
    public long StoreId { get; set; }
    public string FileName { get; set; } = null!;
    public int TotalRows { get; set; }
    public int SuccessCount { get; set; }
    public int FailCount { get; set; }
    public ImportStatus Status { get; set; } = ImportStatus.Pending;
    /// <summary>S3 key of the downloadable error-row CSV, if any rows failed.</summary>
    public string? ErrorReportUrl { get; set; }
    public DateTime CreatedAt { get; set; }

    public Retailer Retailer { get; set; } = null!;
    public ICollection<StoreProduct> Products { get; set; } = new List<StoreProduct>();
}
