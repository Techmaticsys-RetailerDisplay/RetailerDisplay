using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailerDisplay.Application.Admin;
using RetailerDisplay.Application.Catalog;
using RetailerDisplay.Application.Common;

namespace RetailerDisplay.Api.Controllers;

[ApiController]
[Route("api/v1/admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _admin;

    public AdminController(IAdminService admin) => _admin = admin;

    [HttpPost("auth/login")]
    public async Task<ActionResult<AdminAuthResponse>> Login(AdminLoginRequest request, CancellationToken ct)
        => Ok(await _admin.LoginAsync(request, ct));

    [Authorize(Roles = "Admin")]
    [HttpGet("overview")]
    public async Task<ActionResult<PlatformOverview>> Overview(CancellationToken ct)
        => Ok(await _admin.GetOverviewAsync(ct));

    [Authorize(Roles = "Admin")]
    [HttpGet("retailers")]
    public async Task<ActionResult<IReadOnlyList<RetailerUsage>>> Retailers(CancellationToken ct)
        => Ok(await _admin.ListRetailersAsync(ct));

    [Authorize(Roles = "Admin")]
    [HttpPost("retailers")]
    public async Task<ActionResult<RetailerUsage>> CreateRetailer(CreateRetailerRequest request, CancellationToken ct)
        => Ok(await _admin.CreateRetailerAsync(request, ct));

    [Authorize(Roles = "Admin")]
    [HttpGet("master-products")]
    public async Task<ActionResult<PagedResult<MasterProductDto>>> MasterProducts(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string? search = null, CancellationToken ct = default)
        => Ok(await _admin.ListMasterProductsAsync(new PageQuery(page, pageSize, search), ct));

    [Authorize(Roles = "Admin")]
    [HttpPost("master-products/import")]
    public async Task<ActionResult<ImportResultDto>> ImportMaster(IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0) throw new AppException("A CSV file is required.");
        await using var stream = file.OpenReadStream();
        return Ok(await _admin.ImportMasterCsvAsync(stream, file.FileName, ct));
    }
}
