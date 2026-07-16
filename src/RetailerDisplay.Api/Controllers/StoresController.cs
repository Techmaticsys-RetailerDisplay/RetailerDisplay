using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailerDisplay.Application.Common;
using RetailerDisplay.Application.Stores;

namespace RetailerDisplay.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/stores")]
public class StoresController : ControllerBase
{
    private readonly IStoreService _stores;
    private readonly ICurrentUser _currentUser;

    public StoresController(IStoreService stores, ICurrentUser currentUser)
    {
        _stores = stores;
        _currentUser = currentUser;
    }

    private long RetailerId => _currentUser.RequireRetailerId();

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StoreDto>>> List(CancellationToken ct)
        => Ok(await _stores.ListAsync(RetailerId, ct));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<StoreDto>> Get(long id, CancellationToken ct)
        => Ok(await _stores.GetAsync(RetailerId, id, ct));

    [HttpPost]
    public async Task<ActionResult<StoreDto>> Create(CreateStoreRequest request, CancellationToken ct)
        => Ok(await _stores.CreateAsync(RetailerId, request, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<StoreDto>> Update(long id, UpdateStoreRequest request, CancellationToken ct)
        => Ok(await _stores.UpdateAsync(RetailerId, id, request, ct));

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Deactivate(long id, CancellationToken ct)
    {
        await _stores.DeactivateAsync(RetailerId, id, ct);
        return NoContent();
    }
}
