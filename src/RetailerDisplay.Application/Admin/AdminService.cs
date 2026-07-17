using Microsoft.EntityFrameworkCore;
using RetailerDisplay.Application.Catalog;
using RetailerDisplay.Application.Common;
using RetailerDisplay.Application.Common.Security;
using RetailerDisplay.Application.Devices;
using RetailerDisplay.Domain.Entities;

namespace RetailerDisplay.Application.Admin;

/// <summary>
/// Super-admin operations. Deliberately NOT tenant-scoped — reads across all retailers.
/// </summary>
public class AdminService : IAdminService
{
    private readonly IApplicationDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _tokens;
    private readonly ICsvProductImporter _importer;

    public AdminService(IApplicationDbContext db, IPasswordHasher passwordHasher, IJwtTokenService tokens, ICsvProductImporter importer)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _tokens = tokens;
        _importer = importer;
    }

    public async Task<AdminAuthResponse> LoginAsync(AdminLoginRequest request, CancellationToken ct = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var admin = await _db.AdminUsers.FirstOrDefaultAsync(a => a.Email == email, ct);
        if (admin is null || !admin.IsActive || !_passwordHasher.Verify(request.Password, admin.PasswordHash))
            throw AppException.Unauthorized("Invalid email or password.");

        var (token, expires) = _tokens.CreateAdminAccessToken(admin);
        return new AdminAuthResponse(token, expires, new AdminProfile(admin.AdminUserId, admin.Email, admin.Name));
    }

    public async Task<PlatformOverview> GetOverviewAsync(CancellationToken ct = default)
    {
        var lastSeen = await _db.Devices.Select(d => d.LastSeenAt).ToListAsync(ct);
        var online = lastSeen.Count(DeviceStatusRules.IsOnline);

        return new PlatformOverview(
            TotalRetailers: await _db.Retailers.CountAsync(ct),
            TotalStores: await _db.Stores.CountAsync(ct),
            TotalDevices: lastSeen.Count,
            DevicesOnline: online,
            DevicesOffline: lastSeen.Count - online,
            TotalProducts: await _db.StoreProducts.CountAsync(ct),
            TotalPlaylists: await _db.Playlists.CountAsync(ct),
            TotalContent: await _db.Contents.CountAsync(ct));
    }

    public async Task<RetailerUsage> CreateRetailerAsync(CreateRetailerRequest request, CancellationToken ct = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (await _db.Retailers.AnyAsync(r => r.Email == email, ct))
            throw AppException.Conflict("A retailer with this email already exists.");

        var now = DateTime.UtcNow;
        var retailer = new Domain.Entities.Retailer
        {
            BusinessName = request.BusinessName.Trim(),
            Email = email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            ProfileCompleted = false,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };
        _db.Retailers.Add(retailer);
        await _db.SaveChangesAsync(ct);

        // Auto-provision a default store so single-store retailers skip store setup.
        _db.Stores.Add(new Domain.Entities.Store
        {
            RetailerId = retailer.RetailerId,
            StoreName = retailer.BusinessName,
            TimeZone = "UTC",
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        });
        await _db.SaveChangesAsync(ct);

        return new RetailerUsage(retailer.RetailerId, retailer.BusinessName, retailer.Email, 1, 0, 0, retailer.IsActive, retailer.CreatedAt);
    }

    public async Task<ImportResultDto> ImportMasterCsvAsync(Stream csv, string fileName, CancellationToken ct = default)
    {
        var rows = _importer.Parse(csv, requirePrice: false);
        var now = DateTime.UtcNow;

        var skus = rows.Where(r => r.IsValid && !string.IsNullOrWhiteSpace(r.Sku)).Select(r => r.Sku!.Trim()).ToList();
        var existing = await _db.MasterProducts
            .Where(m => m.Sku != null && skus.Contains(m.Sku))
            .ToDictionaryAsync(m => m.Sku!, ct);

        var errors = new List<ImportRowError>();
        var success = 0;

        foreach (var row in rows)
        {
            if (!row.IsValid) { errors.Add(new ImportRowError(row.RowNumber, row.Error ?? "Invalid row.")); continue; }
            var sku = row.Sku!.Trim();

            if (!existing.TryGetValue(sku, out var m))
            {
                m = new MasterProduct { Sku = sku, CreatedAt = now, IsActive = true };
                _db.MasterProducts.Add(m);
                existing[sku] = m;
            }
            m.ProductName = row.ProductName!.Trim();
            m.Description = row.Description;
            m.Category = row.Category;
            m.Brand = row.Brand;
            m.ProductType = row.ProductType;
            m.Abv = row.Abv;
            m.ContainerType = row.ContainerType;
            m.Volume = row.Volume;
            m.PackSize = row.PackSize;
            m.Vintage = row.Vintage;
            m.IsActive = true;
            m.UpdatedAt = now;
            success++;
        }

        await _db.SaveChangesAsync(ct);
        return new ImportResultDto(0, rows.Count, success, errors.Count, "Completed", errors);
    }

    public async Task<PagedResult<MasterProductDto>> ListMasterProductsAsync(PageQuery query, CancellationToken ct = default)
    {
        var q = _db.MasterProducts.AsQueryable();
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            q = q.Where(m => m.ProductName.ToLower().Contains(term) || (m.Sku != null && m.Sku.ToLower().Contains(term)));
        }
        var total = await q.CountAsync(ct);
        var items = await q.OrderBy(m => m.ProductName)
            .Skip(query.Skip).Take(query.SafePageSize)
            .Select(m => new MasterProductDto(m.MasterProductId, m.Sku, m.Upc, m.ProductName, m.Category, m.Brand,
                m.ProductType, m.Abv, m.ContainerType, m.Volume, m.PackSize, m.Vintage, m.DefaultImageUrl, m.IsActive))
            .ToListAsync(ct);
        return new PagedResult<MasterProductDto>(items, total, query.SafePage, query.SafePageSize);
    }

    public async Task<IReadOnlyList<RetailerUsage>> ListRetailersAsync(CancellationToken ct = default)
    {
        return await _db.Retailers
            .OrderByDescending(r => r.RetailerId)
            .Select(r => new RetailerUsage(
                r.RetailerId,
                r.BusinessName,
                r.Email,
                r.Stores.Count,
                r.Devices.Count,
                r.Products.Count,
                r.IsActive,
                r.CreatedAt))
            .ToListAsync(ct);
    }
}
