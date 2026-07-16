using Microsoft.AspNetCore.Mvc;
using RetailerDisplay.Application.Common;
using RetailerDisplay.Application.DeviceApi;

namespace RetailerDisplay.Api.Controllers;

/// <summary>
/// Device-facing API. Authenticates by the device key sent in the X-Device-Key header
/// (except pair, which takes the key in the body). No retailer JWT here.
/// </summary>
[ApiController]
[Route("api/v1/device")]
public class DeviceController : ControllerBase
{
    private const string KeyHeader = "X-Device-Key";
    private readonly IDeviceApiService _deviceApi;

    public DeviceController(IDeviceApiService deviceApi) => _deviceApi = deviceApi;

    [HttpPost("pair")]
    public async Task<ActionResult<PairResponse>> Pair(PairRequest request, CancellationToken ct)
        => Ok(await _deviceApi.PairAsync(request, ct));

    [HttpGet("manifest")]
    public async Task<ActionResult<ManifestResponse>> Manifest([FromQuery] int? knownVersion, CancellationToken ct)
    {
        var key = ReadKey();
        var manifest = await _deviceApi.GetManifestAsync(key, knownVersion, ct);
        return manifest is null ? StatusCode(StatusCodes.Status304NotModified) : Ok(manifest);
    }

    [HttpPost("heartbeat")]
    public async Task<ActionResult<HeartbeatResponse>> Heartbeat(HeartbeatRequest request, CancellationToken ct)
        => Ok(await _deviceApi.HeartbeatAsync(ReadKey(), request, ct));

    private string ReadKey()
    {
        var key = Request.Headers[KeyHeader].ToString();
        if (string.IsNullOrWhiteSpace(key))
            throw AppException.Unauthorized($"Missing {KeyHeader} header.");
        return key;
    }
}
