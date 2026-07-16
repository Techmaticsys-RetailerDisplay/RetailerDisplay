namespace RetailerDisplay.Application.DeviceApi;

public interface IDeviceApiService
{
    Task<PairResponse> PairAsync(PairRequest request, CancellationToken ct = default);

    /// <summary>
    /// Returns the device's playlist manifest, or null when the caller's knownVersion already
    /// matches and no refresh is pending (the controller then replies 304 Not Modified).
    /// </summary>
    Task<ManifestResponse?> GetManifestAsync(string deviceKey, int? knownVersion, CancellationToken ct = default);

    Task<HeartbeatResponse> HeartbeatAsync(string deviceKey, HeartbeatRequest request, CancellationToken ct = default);
}
