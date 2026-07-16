namespace RetailerDisplay.Infrastructure.Storage;

/// <summary>Bound from the "S3" configuration section.</summary>
public class S3Options
{
    public string Bucket { get; set; } = "retailer-display";
    public string Region { get; set; } = "us-east-1";
    /// <summary>Custom endpoint for S3-compatible storage (MinIO in dev). Empty = real AWS.</summary>
    public string? ServiceUrl { get; set; }
    public string? AccessKey { get; set; }
    public string? SecretKey { get; set; }
    /// <summary>Required for MinIO / path-style buckets.</summary>
    public bool ForcePathStyle { get; set; }
}
