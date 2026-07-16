using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailerDisplay.Application.Common;
using RetailerDisplay.Application.Devices;

namespace RetailerDisplay.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/devices")]
public class DevicesController : ControllerBase
{
    private readonly IDeviceService _devices;
    private readonly ICurrentUser _currentUser;

    public DevicesController(IDeviceService devices, ICurrentUser currentUser)
    {
        _devices = devices;
        _currentUser = currentUser;
    }

    private long RetailerId => _currentUser.RequireRetailerId();

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DeviceDto>>> List(CancellationToken ct)
        => Ok(await _devices.ListAsync(RetailerId, ct));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<DeviceDto>> Get(long id, CancellationToken ct)
        => Ok(await _devices.GetAsync(RetailerId, id, ct));

    [HttpPost]
    public async Task<ActionResult<DeviceDto>> Register(RegisterDeviceRequest request, CancellationToken ct)
        => Ok(await _devices.RegisterAsync(RetailerId, request, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<DeviceDto>> Update(long id, UpdateDeviceRequest request, CancellationToken ct)
        => Ok(await _devices.UpdateAsync(RetailerId, id, request, ct));

    [HttpPut("{id:long}/playlist")]
    public async Task<ActionResult<DeviceDto>> AssignPlaylist(long id, AssignPlaylistRequest request, CancellationToken ct)
        => Ok(await _devices.AssignPlaylistAsync(RetailerId, id, request, ct));

    [HttpPost("{id:long}/refresh")]
    public async Task<IActionResult> Refresh(long id, CancellationToken ct)
    {
        await _devices.RequestRefreshAsync(RetailerId, id, ct);
        return NoContent();
    }

    [HttpPost("{id:long}/revoke")]
    public async Task<IActionResult> Revoke(long id, CancellationToken ct)
    {
        await _devices.RevokeAsync(RetailerId, id, ct);
        return NoContent();
    }

    [HttpGet("{id:long}/status-log")]
    public async Task<ActionResult<IReadOnlyList<DeviceStatusLogDto>>> StatusLog(long id, CancellationToken ct)
        => Ok(await _devices.GetStatusLogAsync(RetailerId, id, ct));

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        await _devices.DeleteAsync(RetailerId, id, ct);
        return NoContent();
    }
}
