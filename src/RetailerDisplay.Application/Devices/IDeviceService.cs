namespace RetailerDisplay.Application.Devices;

public interface IDeviceService
{
    Task<IReadOnlyList<DeviceDto>> ListAsync(long retailerId, CancellationToken ct = default);
    Task<DeviceDto> GetAsync(long retailerId, long deviceId, CancellationToken ct = default);
    Task<DeviceDto> RegisterAsync(long retailerId, RegisterDeviceRequest request, CancellationToken ct = default);
    Task<DeviceDto> UpdateAsync(long retailerId, long deviceId, UpdateDeviceRequest request, CancellationToken ct = default);
    Task<DeviceDto> AssignPlaylistAsync(long retailerId, long deviceId, AssignPlaylistRequest request, CancellationToken ct = default);
    Task RequestRefreshAsync(long retailerId, long deviceId, CancellationToken ct = default);
    Task RevokeAsync(long retailerId, long deviceId, CancellationToken ct = default);
    Task<IReadOnlyList<DeviceStatusLogDto>> GetStatusLogAsync(long retailerId, long deviceId, CancellationToken ct = default);
    Task DeleteAsync(long retailerId, long deviceId, CancellationToken ct = default);
}
