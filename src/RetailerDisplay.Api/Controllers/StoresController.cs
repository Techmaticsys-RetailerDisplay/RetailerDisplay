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

    // Retailers can view (and edit details of) their stores, but not create or remove them —
    // stores are provisioned for the account (auto-default now, admin-managed later).
    [HttpPut("{id:long}")]
    public async Task<ActionResult<StoreDto>> Update(long id, UpdateStoreRequest request, CancellationToken ct)
        => Ok(await _stores.UpdateAsync(RetailerId, id, request, ct));
}
