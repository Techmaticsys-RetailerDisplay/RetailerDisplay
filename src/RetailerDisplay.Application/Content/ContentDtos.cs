using RetailerDisplay.Domain.Enums;

namespace RetailerDisplay.Application.Content;

public record ContentDto(
    long ContentId,
    string ContentType,
    string Name,
    string? MasterKey,
    string? MediaVariants,
    string? ThumbnailKey,
    int? DurationSeconds,
    int Version,
    bool IsActive,
    IReadOnlyList<ContentProductDto>? Products);

public record ContentProductDto(long StoreProductId, string ProductName, decimal Price, string? ImageUrl, int SortOrder);

/// <summary>
/// Create content. For Image/Video, supply the uploaded object's MasterKey (from the
/// presigned upload). For ProductList, supply the picked StoreProductIds in order.
/// </summary>
public record CreateContentRequest(
    ContentType ContentType,
    string Name,
    string? MasterKey,
    long? FileSizeBytes,
    int? DurationSeconds,
    IReadOnlyList<long>? StoreProductIds);

public record UpdateContentRequest(string Name, bool IsActive);

public record SetContentProductsRequest(IReadOnlyList<long> StoreProductIds);
