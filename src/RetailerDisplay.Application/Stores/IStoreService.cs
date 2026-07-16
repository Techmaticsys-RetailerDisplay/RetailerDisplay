namespace RetailerDisplay.Application.Stores;

public interface IStoreService
{
    Task<IReadOnlyList<StoreDto>> ListAsync(long retailerId, CancellationToken ct = default);
    Task<StoreDto> GetAsync(long retailerId, long storeId, CancellationToken ct = default);
    Task<StoreDto> CreateAsync(long retailerId, CreateStoreRequest request, CancellationToken ct = default);
    Task<StoreDto> UpdateAsync(long retailerId, long storeId, UpdateStoreRequest request, CancellationToken ct = default);
    Task DeactivateAsync(long retailerId, long storeId, CancellationToken ct = default);
}
