namespace RetailerDisplay.Application.Catalog;

public record MasterProductDto(
    long MasterProductId,
    string? Sku,
    string? Upc,
    string ProductName,
    string? Category,
    string? Brand,
    string? ProductType,
    decimal? Abv,
    string? ContainerType,
    string? Volume,
    int? PackSize,
    int? Vintage,
    string? DefaultImageUrl,
    bool IsActive);

public record StoreProductDto(
    long StoreProductId,
    long StoreId,
    long? MasterProductId,
    string Source,
    string Sku,
    string ProductName,
    string? Description,
    string? Category,
    string? Brand,
    string? ProductType,
    decimal? Abv,
    string? ContainerType,
    string? Volume,
    int? PackSize,
    int? Vintage,
    decimal Price,
    decimal? SalePrice,
    string Currency,
    string? ImageUrl,
    bool IsActive);

public record CreateStoreProductRequest(
    string Sku,
    string ProductName,
    string? Description,
    string? Category,
    string? Brand,
    string? ProductType,
    decimal? Abv,
    string? ContainerType,
    string? Volume,
    int? PackSize,
    int? Vintage,
    decimal Price,
    decimal? SalePrice,
    string? Currency,
    string? ImageUrl);

public record UpdateStoreProductRequest(
    string ProductName,
    string? Description,
    string? Category,
    string? Brand,
    string? ProductType,
    decimal? Abv,
    string? ContainerType,
    string? Volume,
    int? PackSize,
    int? Vintage,
    decimal Price,
    decimal? SalePrice,
    string? Currency,
    string? ImageUrl,
    bool IsActive);

public record PullFromMasterRequest(IReadOnlyList<long> MasterProductIds);

public record ImportResultDto(
    long ImportId,
    int TotalRows,
    int SuccessCount,
    int FailCount,
    string Status,
    IReadOnlyList<ImportRowError> Errors);

public record ImportRowError(int RowNumber, string Message);
