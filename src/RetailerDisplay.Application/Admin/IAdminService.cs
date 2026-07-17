using RetailerDisplay.Application.Catalog;
using RetailerDisplay.Application.Common;

namespace RetailerDisplay.Application.Admin;

public interface IAdminService
{
    Task<AdminAuthResponse> LoginAsync(AdminLoginRequest request, CancellationToken ct = default);
    Task<PlatformOverview> GetOverviewAsync(CancellationToken ct = default);
    Task<IReadOnlyList<RetailerUsage>> ListRetailersAsync(CancellationToken ct = default);
    Task<RetailerUsage> CreateRetailerAsync(CreateRetailerRequest request, CancellationToken ct = default);

    Task<ImportResultDto> ImportMasterCsvAsync(Stream csv, string fileName, CancellationToken ct = default);
    Task<PagedResult<MasterProductDto>> ListMasterProductsAsync(PageQuery query, CancellationToken ct = default);
}
