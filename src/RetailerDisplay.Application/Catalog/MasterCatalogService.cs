using Microsoft.EntityFrameworkCore;
using RetailerDisplay.Application.Common;

namespace RetailerDisplay.Application.Catalog;

public class MasterCatalogService : IMasterCatalogService
{
    private readonly IApplicationDbContext _db;

    public MasterCatalogService(IApplicationDbContext db) => _db = db;

    public async Task<PagedResult<MasterProductDto>> BrowseAsync(PageQuery query, CancellationToken ct = default)
    {
        var q = _db.MasterProducts.Where(m => m.IsActive);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            q = q.Where(m =>
                m.ProductName.ToLower().Contains(term) ||
                (m.Brand != null && m.Brand.ToLower().Contains(term)) ||
                (m.Sku != null && m.Sku.ToLower().Contains(term)) ||
                (m.Upc != null && m.Upc.ToLower().Contains(term)));
        }

        var total = await q.CountAsync(ct);
        var items = await q
            .OrderBy(m => m.ProductName)
            .Skip(query.Skip).Take(query.SafePageSize)
            .Select(m => new MasterProductDto(
                m.MasterProductId, m.Sku, m.Upc, m.ProductName, m.Category, m.Brand,
                m.ProductType, m.Abv, m.ContainerType, m.Volume, m.PackSize, m.Vintage,
                m.DefaultImageUrl, m.IsActive))
            .ToListAsync(ct);

        return new PagedResult<MasterProductDto>(items, total, query.SafePage, query.SafePageSize);
    }

    public async Task<MasterProductDto> GetAsync(long masterProductId, CancellationToken ct = default)
    {
        var m = await _db.MasterProducts.FirstOrDefaultAsync(x => x.MasterProductId == masterProductId, ct)
            ?? throw AppException.NotFound("Master product");
        return new MasterProductDto(
            m.MasterProductId, m.Sku, m.Upc, m.ProductName, m.Category, m.Brand,
            m.ProductType, m.Abv, m.ContainerType, m.Volume, m.PackSize, m.Vintage,
            m.DefaultImageUrl, m.IsActive);
    }
}
