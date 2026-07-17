namespace RetailerDisplay.Application.Admin;

public record AdminLoginRequest(string Email, string Password);

public record AdminProfile(long AdminUserId, string Email, string? Name);

public record AdminAuthResponse(string AccessToken, DateTime AccessTokenExpiresAt, AdminProfile Admin);

public record CreateRetailerRequest(string BusinessName, string Email, string Password);

public record PlatformOverview(
    int TotalRetailers,
    int TotalStores,
    int TotalDevices,
    int DevicesOnline,
    int DevicesOffline,
    int TotalProducts,
    int TotalPlaylists,
    int TotalContent);

public record RetailerUsage(
    long RetailerId,
    string BusinessName,
    string Email,
    int StoreCount,
    int DeviceCount,
    int ProductCount,
    bool IsActive,
    DateTime CreatedAt);
