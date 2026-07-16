using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailerDisplay.Application.Catalog;
using RetailerDisplay.Application.Common;

namespace RetailerDisplay.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _products;
    private readonly ICurrentUser _currentUser;

    public ProductsController(IProductService products, ICurrentUser currentUser)
    {
        _products = products;
        _currentUser = currentUser;
    }

    private long RetailerId => _currentUser.RequireRetailerId();

    [HttpGet("stores/{storeId:long}/products")]
    public async Task<ActionResult<PagedResult<StoreProductDto>>> List(
        long storeId, [FromQuery] int page = 1, [FromQuery] int pageSize = 25,
        [FromQuery] string? search = null, [FromQuery] bool activeOnly = false, CancellationToken ct = default)
        => Ok(await _products.ListAsync(RetailerId, storeId, new PageQuery(page, pageSize, search), activeOnly, ct));

    [HttpGet("products/{id:long}")]
    public async Task<ActionResult<StoreProductDto>> Get(long id, CancellationToken ct)
        => Ok(await _products.GetAsync(RetailerId, id, ct));

    [HttpPost("stores/{storeId:long}/products")]
    public async Task<ActionResult<StoreProductDto>> Create(long storeId, CreateStoreProductRequest request, CancellationToken ct)
        => Ok(await _products.CreateAsync(RetailerId, storeId, request, ct));

    [HttpPut("products/{id:long}")]
    public async Task<ActionResult<StoreProductDto>> Update(long id, UpdateStoreProductRequest request, CancellationToken ct)
        => Ok(await _products.UpdateAsync(RetailerId, id, request, ct));

    [HttpPatch("products/{id:long}/active")]
    public async Task<IActionResult> SetActive(long id, [FromQuery] bool value, CancellationToken ct)
    {
        await _products.SetActiveAsync(RetailerId, id, value, ct);
        return NoContent();
    }

    [HttpDelete("products/{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        await _products.DeleteAsync(RetailerId, id, ct);
        return NoContent();
    }

    [HttpPost("stores/{storeId:long}/products/import")]
    public async Task<ActionResult<ImportResultDto>> Import(long storeId, IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            throw new AppException("A CSV file is required.");
        await using var stream = file.OpenReadStream();
        return Ok(await _products.ImportCsvAsync(RetailerId, storeId, stream, file.FileName, ct));
    }

    [HttpPost("stores/{storeId:long}/products/pull")]
    public async Task<ActionResult<object>> Pull(long storeId, PullFromMasterRequest request, CancellationToken ct)
    {
        var pulled = await _products.PullFromMasterAsync(RetailerId, storeId, request, ct);
        return Ok(new { pulled });
    }
}
