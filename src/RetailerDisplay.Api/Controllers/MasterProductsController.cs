using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailerDisplay.Application.Catalog;
using RetailerDisplay.Application.Common;

namespace RetailerDisplay.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/master-products")]
public class MasterProductsController : ControllerBase
{
    private readonly IMasterCatalogService _master;

    public MasterProductsController(IMasterCatalogService master) => _master = master;

    [HttpGet]
    public async Task<ActionResult<PagedResult<MasterProductDto>>> Browse(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 25, [FromQuery] string? search = null,
        CancellationToken ct = default)
        => Ok(await _master.BrowseAsync(new PageQuery(page, pageSize, search), ct));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<MasterProductDto>> Get(long id, CancellationToken ct)
        => Ok(await _master.GetAsync(id, ct));
}
