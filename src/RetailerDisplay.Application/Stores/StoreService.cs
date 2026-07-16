using Microsoft.EntityFrameworkCore;
using RetailerDisplay.Application.Common;
using RetailerDisplay.Domain.Entities;

namespace RetailerDisplay.Application.Stores;

public class StoreService : IStoreService
{
    private readonly IApplicationDbContext _db;

    public StoreService(IApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<StoreDto>> ListAsync(long retailerId, CancellationToken ct = default)
    {
        return await _db.Stores
            .Where(s => s.RetailerId == retailerId)
            .OrderBy(s => s.StoreName)
            .Select(s => Map(s))
            .ToListAsync(ct);
    }

    public async Task<StoreDto> GetAsync(long retailerId, long storeId, CancellationToken ct = default)
    {
        var store = await Find(retailerId, storeId, ct);
        return Map(store);
    }

    public async Task<StoreDto> CreateAsync(long retailerId, CreateStoreRequest r, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var store = new Store
        {
            RetailerId = retailerId,
            StoreName = r.StoreName.Trim(),
            StoreCode = r.StoreCode?.Trim(),
            AddressLine1 = r.AddressLine1,
            AddressLine2 = r.AddressLine2,
            City = r.City,
            State = r.State,
            PostalCode = r.PostalCode,
            Country = r.Country,
            Phone = r.Phone,
            Email = r.Email,
            TimeZone = string.IsNullOrWhiteSpace(r.TimeZone) ? "UTC" : r.TimeZone,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };
        _db.Stores.Add(store);
        await _db.SaveChangesAsync(ct);
        return Map(store);
    }

    public async Task<StoreDto> UpdateAsync(long retailerId, long storeId, UpdateStoreRequest r, CancellationToken ct = default)
    {
        var store = await Find(retailerId, storeId, ct);
        store.StoreName = r.StoreName.Trim();
        store.StoreCode = r.StoreCode?.Trim();
        store.AddressLine1 = r.AddressLine1;
        store.AddressLine2 = r.AddressLine2;
        store.City = r.City;
        store.State = r.State;
        store.PostalCode = r.PostalCode;
        store.Country = r.Country;
        store.Phone = r.Phone;
        store.Email = r.Email;
        store.TimeZone = string.IsNullOrWhiteSpace(r.TimeZone) ? "UTC" : r.TimeZone;
        store.IsActive = r.IsActive;
        store.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return Map(store);
    }

    public async Task DeactivateAsync(long retailerId, long storeId, CancellationToken ct = default)
    {
        var store = await Find(retailerId, storeId, ct);
        store.IsActive = false;
        store.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    private async Task<Store> Find(long retailerId, long storeId, CancellationToken ct) =>
        await _db.Stores.FirstOrDefaultAsync(s => s.RetailerId == retailerId && s.StoreId == storeId, ct)
            ?? throw AppException.NotFound("Store");

    private static StoreDto Map(Store s) => new(
        s.StoreId, s.StoreName, s.StoreCode, s.AddressLine1, s.AddressLine2, s.City, s.State,
        s.PostalCode, s.Country, s.Phone, s.Email, s.TimeZone, s.IsActive);
}
