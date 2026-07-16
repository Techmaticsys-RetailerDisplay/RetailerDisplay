namespace RetailerDisplay.Application.Devices;

public record DeviceDto(
    long DeviceId,
    string DeviceKey,
    string? DeviceName,
    long? StoreId,
    string? StoreName,
    long? PlaylistId,
    string? PlaylistName,
    string Status,
    DateTime? LastSeenAt,
    string? AppVersion,
    string? Orientation,
    int? ScreenWidth,
    int? ScreenHeight,
    bool IsRevoked,
    bool IsActive);

public record RegisterDeviceRequest(string? DeviceName, long? StoreId, long? PlaylistId);

public record UpdateDeviceRequest(string? DeviceName, long? StoreId);

public record AssignPlaylistRequest(long? PlaylistId);

public record DeviceStatusLogDto(string Status, DateTime ChangedAt);
