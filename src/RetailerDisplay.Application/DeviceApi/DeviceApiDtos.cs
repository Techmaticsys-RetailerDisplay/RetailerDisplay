namespace RetailerDisplay.Application.DeviceApi;

public record PairRequest(string DeviceKey, int? ScreenWidth, int? ScreenHeight, string? Orientation, string? AppVersion);

public record PairResponse(long DeviceId, string? DeviceName, long? PlaylistId, int PlaylistVersion);

public record HeartbeatRequest(int? PlaylistVersion, string? AppVersion, int? ScreenWidth, int? ScreenHeight, string? Orientation);

public record HeartbeatResponse(int PlaylistVersion, bool RefreshRequested);

public record ManifestResponse(int Version, bool RefreshRequested, IReadOnlyList<ManifestItem> Items);

public record ManifestItem(
    long ContentId,
    string Type,
    string? Url,
    int DurationSeconds,
    string FitMode,
    string? ContentHash,
    IReadOnlyList<ManifestProduct>? Products);

public record ManifestProduct(string Name, decimal Price, decimal? SalePrice, string? ImageUrl);
