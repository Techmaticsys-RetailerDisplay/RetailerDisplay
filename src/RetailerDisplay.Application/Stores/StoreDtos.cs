namespace RetailerDisplay.Application.Stores;

public record StoreDto(
    long StoreId,
    string StoreName,
    string? StoreCode,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? State,
    string? PostalCode,
    string? Country,
    string? Phone,
    string? Email,
    string TimeZone,
    bool IsActive);

public record CreateStoreRequest(
    string StoreName,
    string? StoreCode,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? State,
    string? PostalCode,
    string? Country,
    string? Phone,
    string? Email,
    string TimeZone);

public record UpdateStoreRequest(
    string StoreName,
    string? StoreCode,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? State,
    string? PostalCode,
    string? Country,
    string? Phone,
    string? Email,
    string TimeZone,
    bool IsActive);
