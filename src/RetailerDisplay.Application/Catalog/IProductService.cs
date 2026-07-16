using RetailerDisplay.Application.Common;

namespace RetailerDisplay.Application.Catalog;

public interface IProductService
{
    Task<PagedResult<StoreProductDto>> ListAsync(long retailerId, long storeId, PageQuery query, bool activeOnly, CancellationToken ct = default);
    Task<StoreProductDto> GetAsync(long retailerId, long productId, CancellationToken ct = default);
    Task<StoreProductDto> CreateAsync(long retailerId, long storeId, CreateStoreProductRequest request, CancellationToken ct = default);
    Task<StoreProductDto> UpdateAsync(long retailerId, long productId, UpdateStoreProductRequest request, CancellationToken ct = default);
    Task SetActiveAsync(long retailerId, long productId, bool isActive, CancellationToken ct = default);
    Task DeleteAsync(long retailerId, long productId, CancellationToken ct = default);

    Task<ImportResultDto> ImportCsvAsync(long retailerId, long storeId, Stream csv, string fileName, CancellationToken ct = default);
    Task<int> PullFromMasterAsync(long retailerId, long storeId, PullFromMasterRequest request, CancellationToken ct = default);
}
