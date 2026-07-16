using Microsoft.EntityFrameworkCore;
using RetailerDisplay.Application.Common;
using RetailerDisplay.Domain.Entities;
using RetailerDisplay.Domain.Enums;

namespace RetailerDisplay.Application.Catalog;

public class ProductService : IProductService
{
    private readonly IApplicationDbContext _db;
    private readonly ICsvProductImporter _importer;

    public ProductService(IApplicationDbContext db, ICsvProductImporter importer)
    {
        _db = db;
        _importer = importer;
    }

    public async Task<PagedResult<StoreProductDto>> ListAsync(long retailerId, long storeId, PageQuery query, bool activeOnly, CancellationToken ct = default)
    {
        await EnsureStore(retailerId, storeId, ct);

        var q = _db.StoreProducts.Where(p => p.RetailerId == retailerId && p.StoreId == storeId);
        if (activeOnly) q = q.Where(p => p.IsActive);
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            q = q.Where(p => p.ProductName.ToLower().Contains(term) || p.Sku.ToLower().Contains(term));
        }

        var total = await q.CountAsync(ct);
        var items = await q
            .OrderBy(p => p.ProductName)
            .Skip(query.Skip).Take(query.SafePageSize)
            .Select(p => Map(p))
            .ToListAsync(ct);

        return new PagedResult<StoreProductDto>(items, total, query.SafePage, query.SafePageSize);
    }

    public async Task<StoreProductDto> GetAsync(long retailerId, long productId, CancellationToken ct = default)
        => Map(await Find(retailerId, productId, ct));

    public async Task<StoreProductDto> CreateAsync(long retailerId, long storeId, CreateStoreProductRequest r, CancellationToken ct = default)
    {
        await EnsureStore(retailerId, storeId, ct);

        var sku = r.Sku.Trim();
        if (await _db.StoreProducts.AnyAsync(p => p.StoreId == storeId && p.Sku == sku, ct))
            throw AppException.Conflict($"A product with SKU '{sku}' already exists in this store.");

        var now = DateTime.UtcNow;
        var product = new StoreProduct
        {
            RetailerId = retailerId,
            StoreId = storeId,
            Source = ProductSource.Manual,
            Sku = sku,
            ProductName = r.ProductName.Trim(),
            Description = r.Description,
            Category = r.Category,
            Brand = r.Brand,
            ProductType = r.ProductType,
            Abv = r.Abv,
            ContainerType = r.ContainerType,
            Volume = r.Volume,
            PackSize = r.PackSize,
            Vintage = r.Vintage,
            Price = r.Price,
            SalePrice = r.SalePrice,
            Currency = string.IsNullOrWhiteSpace(r.Currency) ? "USD" : r.Currency,
            ImageUrl = r.ImageUrl,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };
        _db.StoreProducts.Add(product);
        await _db.SaveChangesAsync(ct);
        return Map(product);
    }

    public async Task<StoreProductDto> UpdateAsync(long retailerId, long productId, UpdateStoreProductRequest r, CancellationToken ct = default)
    {
        var p = await Find(retailerId, productId, ct);
        p.ProductName = r.ProductName.Trim();
        p.Description = r.Description;
        p.Category = r.Category;
        p.Brand = r.Brand;
        p.ProductType = r.ProductType;
        p.Abv = r.Abv;
        p.ContainerType = r.ContainerType;
        p.Volume = r.Volume;
        p.PackSize = r.PackSize;
        p.Vintage = r.Vintage;
        p.Price = r.Price;
        p.SalePrice = r.SalePrice;
        p.Currency = string.IsNullOrWhiteSpace(r.Currency) ? p.Currency : r.Currency;
        p.ImageUrl = r.ImageUrl;
        p.IsActive = r.IsActive;
        p.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return Map(p);
    }

    public async Task SetActiveAsync(long retailerId, long productId, bool isActive, CancellationToken ct = default)
    {
        var p = await Find(retailerId, productId, ct);
        p.IsActive = isActive;
        p.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(long retailerId, long productId, CancellationToken ct = default)
    {
        var p = await Find(retailerId, productId, ct);
        _db.StoreProducts.Remove(p);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<ImportResultDto> ImportCsvAsync(long retailerId, long storeId, Stream csv, string fileName, CancellationToken ct = default)
    {
        await EnsureStore(retailerId, storeId, ct);

        var rows = _importer.Parse(csv);
        var now = DateTime.UtcNow;

        var batch = new ProductImport
        {
            RetailerId = retailerId,
            StoreId = storeId,
            FileName = fileName,
            TotalRows = rows.Count,
            Status = ImportStatus.Pending,
            CreatedAt = now
        };
        _db.ProductImports.Add(batch);
        await _db.SaveChangesAsync(ct);

        var existing = await _db.StoreProducts
            .Where(p => p.StoreId == storeId)
            .ToDictionaryAsync(p => p.Sku, ct);

        var errors = new List<ImportRowError>();
        var success = 0;

        foreach (var row in rows)
        {
            if (!row.IsValid)
            {
                errors.Add(new ImportRowError(row.RowNumber, row.Error ?? "Invalid row."));
                continue;
            }

            var sku = row.Sku!.Trim();
            if (existing.TryGetValue(sku, out var product))
            {
                // Update existing (upsert)
                product.ProductName = row.ProductName!.Trim();
                product.Description = row.Description;
                product.Category = row.Category;
                product.Brand = row.Brand;
                product.ProductType = row.ProductType;
                product.Abv = row.Abv;
                product.ContainerType = row.ContainerType;
                product.Volume = row.Volume;
                product.PackSize = row.PackSize;
                product.Vintage = row.Vintage;
                product.Price = row.Price;
                product.SalePrice = row.SalePrice;
                product.Currency = string.IsNullOrWhiteSpace(row.Currency) ? product.Currency : row.Currency!;
                product.ImportBatchId = batch.ImportId;
                product.UpdatedAt = now;
            }
            else
            {
                var created = new StoreProduct
                {
                    RetailerId = retailerId,
                    StoreId = storeId,
                    Source = ProductSource.Csv,
                    Sku = sku,
                    ProductName = row.ProductName!.Trim(),
                    Description = row.Description,
                    Category = row.Category,
                    Brand = row.Brand,
                    ProductType = row.ProductType,
                    Abv = row.Abv,
                    ContainerType = row.ContainerType,
                    Volume = row.Volume,
                    PackSize = row.PackSize,
                    Vintage = row.Vintage,
                    Price = row.Price,
                    SalePrice = row.SalePrice,
                    Currency = string.IsNullOrWhiteSpace(row.Currency) ? "USD" : row.Currency!,
                    IsActive = true,
                    ImportBatchId = batch.ImportId,
                    CreatedAt = now,
                    UpdatedAt = now
                };
                _db.StoreProducts.Add(created);
                existing[sku] = created;
            }
            success++;
        }

        batch.SuccessCount = success;
        batch.FailCount = errors.Count;
        batch.Status = ImportStatus.Completed; // partial success is still Completed; Failed is reserved for parse failure
        // NOTE: an error-report CSV upload to S3 (ErrorReportUrl) is a follow-up.
        await _db.SaveChangesAsync(ct);

        return new ImportResultDto(batch.ImportId, batch.TotalRows, success, errors.Count,
            batch.Status.ToString(), errors);
    }

    public async Task<int> PullFromMasterAsync(long retailerId, long storeId, PullFromMasterRequest request, CancellationToken ct = default)
    {
        await EnsureStore(retailerId, storeId, ct);
        if (request.MasterProductIds.Count == 0) return 0;

        var masters = await _db.MasterProducts
            .Where(m => request.MasterProductIds.Contains(m.MasterProductId) && m.IsActive)
            .ToListAsync(ct);

        var alreadyPulled = await _db.StoreProducts
            .Where(p => p.StoreId == storeId && p.MasterProductId != null)
            .Select(p => p.MasterProductId!.Value)
            .ToListAsync(ct);
        var pulledSet = alreadyPulled.ToHashSet();

        var existingSkus = await _db.StoreProducts
            .Where(p => p.StoreId == storeId)
            .Select(p => p.Sku)
            .ToListAsync(ct);
        var skuSet = existingSkus.ToHashSet();

        var now = DateTime.UtcNow;
        var pulled = 0;

        foreach (var m in masters)
        {
            if (pulledSet.Contains(m.MasterProductId)) continue;

            var sku = m.Sku ?? $"M{m.MasterProductId}";
            if (skuSet.Contains(sku)) continue;

            _db.StoreProducts.Add(new StoreProduct
            {
                RetailerId = retailerId,
                StoreId = storeId,
                MasterProductId = m.MasterProductId,
                Source = ProductSource.Master,
                Sku = sku,
                ProductName = m.ProductName,
                Description = m.Description,
                Category = m.Category,
                Brand = m.Brand,
                ProductType = m.ProductType,
                Abv = m.Abv,
                ContainerType = m.ContainerType,
                Volume = m.Volume,
                PackSize = m.PackSize,
                Vintage = m.Vintage,
                Price = 0m,                 // retailer sets store price after pull
                Currency = "USD",
                ImageUrl = m.DefaultImageUrl,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            });
            skuSet.Add(sku);
            pulled++;
        }

        await _db.SaveChangesAsync(ct);
        return pulled;
    }

    private async Task EnsureStore(long retailerId, long storeId, CancellationToken ct)
    {
        var ok = await _db.Stores.AnyAsync(s => s.RetailerId == retailerId && s.StoreId == storeId, ct);
        if (!ok) throw AppException.NotFound("Store");
    }

    private async Task<StoreProduct> Find(long retailerId, long productId, CancellationToken ct) =>
        await _db.StoreProducts.FirstOrDefaultAsync(p => p.RetailerId == retailerId && p.StoreProductId == productId, ct)
            ?? throw AppException.NotFound("Product");

    private static StoreProductDto Map(StoreProduct p) => new(
        p.StoreProductId, p.StoreId, p.MasterProductId, p.Source.ToString(), p.Sku, p.ProductName,
        p.Description, p.Category, p.Brand, p.ProductType, p.Abv, p.ContainerType, p.Volume,
        p.PackSize, p.Vintage, p.Price, p.SalePrice, p.Currency, p.ImageUrl, p.IsActive);
}
