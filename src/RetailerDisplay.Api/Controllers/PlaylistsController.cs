using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailerDisplay.Application.Common;
using RetailerDisplay.Application.Playlists;

namespace RetailerDisplay.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/playlists")]
public class PlaylistsController : ControllerBase
{
    private readonly IPlaylistService _playlists;
    private readonly ICurrentUser _currentUser;

    public PlaylistsController(IPlaylistService playlists, ICurrentUser currentUser)
    {
        _playlists = playlists;
        _currentUser = currentUser;
    }

    private long RetailerId => _currentUser.RequireRetailerId();

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PlaylistDto>>> List(CancellationToken ct)
        => Ok(await _playlists.ListAsync(RetailerId, ct));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<PlaylistDetailDto>> Get(long id, CancellationToken ct)
        => Ok(await _playlists.GetAsync(RetailerId, id, ct));

    [HttpPost]
    public async Task<ActionResult<PlaylistDto>> Create(CreatePlaylistRequest request, CancellationToken ct)
        => Ok(await _playlists.CreateAsync(RetailerId, request, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<PlaylistDto>> Update(long id, UpdatePlaylistRequest request, CancellationToken ct)
        => Ok(await _playlists.UpdateAsync(RetailerId, id, request, ct));

    [HttpPut("{id:long}/items")]
    public async Task<ActionResult<PlaylistDetailDto>> SetItems(long id, SetPlaylistItemsRequest request, CancellationToken ct)
        => Ok(await _playlists.SetItemsAsync(RetailerId, id, request, ct));

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        await _playlists.DeleteAsync(RetailerId, id, ct);
        return NoContent();
    }
}
