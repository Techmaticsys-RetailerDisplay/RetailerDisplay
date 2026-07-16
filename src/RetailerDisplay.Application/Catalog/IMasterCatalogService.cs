using RetailerDisplay.Application.Common;

namespace RetailerDisplay.Application.Catalog;

public interface IMasterCatalogService
{
    Task<PagedResult<MasterProductDto>> BrowseAsync(PageQuery query, CancellationToken ct = default);
    Task<MasterProductDto> GetAsync(long masterProductId, CancellationToken ct = default);
}
