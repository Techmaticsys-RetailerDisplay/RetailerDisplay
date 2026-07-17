using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RetailerDisplay.Application.Common;
using RetailerDisplay.Application.Common.Imaging;
using RetailerDisplay.Application.Common.Storage;
using RetailerDisplay.Domain.Entities;
using RetailerDisplay.Domain.Enums;

namespace RetailerDisplay.Application.Content;

public class ContentService : IContentService
{
    private readonly IApplicationDbContext _db;
    private readonly IMediaStorage _storage;
    private readonly IImageProcessor _imageProcessor;

    public ContentService(IApplicationDbContext db, IMediaStorage storage, IImageProcessor imageProcessor)
    {
        _db = db;
        _storage = storage;
        _imageProcessor = imageProcessor;
    }

    public async Task<IReadOnlyList<ContentDto>> ListAsync(long retailerId, ContentType? type, CancellationToken ct = default)
    {
        var q = _db.Contents.Where(c => c.RetailerId == retailerId);
        if (type.HasValue) q = q.Where(c => c.ContentType == type.Value);
        var items = await q.OrderByDescending(c => c.ContentId).ToListAsync(ct);
        var result = new List<ContentDto>(items.Count);
        foreach (var c in items) result.Add(await MapAsync(c, null, ct));
        return result;
    }

    public async Task<ContentDto> GetAsync(long retailerId, long contentId, CancellationToken ct = default)
    {
        var content = await Find(retailerId, contentId, ct);
        List<ContentProductDto>? products = null;
        if (content.ContentType == ContentType.ProductList)
            products = await LoadProducts(contentId, ct);
        return await MapAsync(content, products, ct);
    }

    public async Task<ContentDto> CreateAsync(long retailerId, CreateContentRequest r, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var content = new Domain.Entities.Content
        {
            RetailerId = retailerId,
            ContentType = r.ContentType,
            Name = r.Name.Trim(),
            FileSizeBytes = r.FileSizeBytes,
            Version = 1,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        switch (r.ContentType)
        {
            case ContentType.Image:
                if (string.IsNullOrWhiteSpace(r.MasterKey))
                    throw new AppException("MasterKey is required for image content.");
                content.MasterKey = r.MasterKey;
                await GenerateImageRenditions(content, ct);
                break;

            case ContentType.Video:
                if (string.IsNullOrWhiteSpace(r.MasterKey))
                    throw new AppException("MasterKey is required for video content.");
                if (r.DurationSeconds is > MediaLimits.MaxVideoSeconds)
                    throw new AppException($"Video exceeds the {MediaLimits.MaxVideoSeconds}s limit.");
                content.MasterKey = r.MasterKey;
                content.DurationSeconds = r.DurationSeconds;
                content.ContentHash = Hash(r.MasterKey);
                break;

            case ContentType.ProductList:
                if (r.StoreProductIds is null || r.StoreProductIds.Count == 0)
                    throw new AppException("A product list needs at least one product.");
                content.ContentHash = Hash(string.Join(",", r.StoreProductIds));
                break;

            default:
                throw new AppException("Unknown content type.");
        }

        _db.Contents.Add(content);
        await _db.SaveChangesAsync(ct);

        if (r.ContentType == ContentType.ProductList)
            await ReplaceProducts(retailerId, content.ContentId, r.StoreProductIds!, ct);

        return await GetAsync(retailerId, content.ContentId, ct);
    }

    public async Task<ContentDto> UpdateAsync(long retailerId, long contentId, UpdateContentRequest r, CancellationToken ct = default)
    {
        var content = await Find(retailerId, contentId, ct);
        content.Name = r.Name.Trim();
        content.IsActive = r.IsActive;
        content.Version += 1;
        content.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return await MapAsync(content, null, ct);
    }

    public async Task SetProductsAsync(long retailerId, long contentId, SetContentProductsRequest r, CancellationToken ct = default)
    {
        var content = await Find(retailerId, contentId, ct);
        if (content.ContentType != ContentType.ProductList)
            throw new AppException("Only product-list content can have products.");

        await ReplaceProducts(retailerId, contentId, r.StoreProductIds, ct);
        content.Version += 1;
        content.ContentHash = Hash(string.Join(",", r.StoreProductIds));
        content.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(long retailerId, long contentId, CancellationToken ct = default)
    {
        var content = await Find(retailerId, contentId, ct);
        var usedInPlaylist = await _db.PlaylistItems.AnyAsync(i => i.ContentId == contentId, ct);
        if (usedInPlaylist)
            throw AppException.Conflict("This content is used in a playlist. Remove it there first.");

        _db.Contents.Remove(content);
        await _db.SaveChangesAsync(ct);
    }

    private async Task GenerateImageRenditions(Domain.Entities.Content content, CancellationToken ct)
    {
        await using var source = await _storage.OpenReadAsync(content.MasterKey!, ct);
        var renditions = await _imageProcessor.CreateRenditionsAsync(source, ct);

        var dir = content.MasterKey!.Contains('/')
            ? content.MasterKey[..content.MasterKey.LastIndexOf('/')]
            : content.MasterKey;

        var variants = new Dictionary<string, string>();
        foreach (var rendition in renditions)
        {
            var key = $"{dir}/rnd_{rendition.Name}.webp";
            using var ms = new MemoryStream(rendition.Data);
            await _storage.SaveAsync(key, ms, rendition.ContentType, ct);
            variants[rendition.Name] = key;
        }

        content.MediaVariants = JsonSerializer.Serialize(variants);
        content.ThumbnailKey = variants.GetValueOrDefault("thumb");
        content.ContentHash = Hash(content.MediaVariants);
    }

    private async Task ReplaceProducts(long retailerId, long contentId, IReadOnlyList<long> productIds, CancellationToken ct)
    {
        var valid = await _db.StoreProducts
            .Where(p => p.RetailerId == retailerId && productIds.Contains(p.StoreProductId))
            .Select(p => p.StoreProductId)
            .ToListAsync(ct);
        var validSet = valid.ToHashSet();

        var existing = await _db.ContentProducts.Where(cp => cp.ContentId == contentId).ToListAsync(ct);
        _db.ContentProducts.RemoveRange(existing);

        var order = 0;
        foreach (var id in productIds)
        {
            if (!validSet.Contains(id)) continue;
            _db.ContentProducts.Add(new ContentProduct
            {
                ContentId = contentId,
                StoreProductId = id,
                SortOrder = order++
            });
        }
        await _db.SaveChangesAsync(ct);
    }

    private async Task<List<ContentProductDto>> LoadProducts(long contentId, CancellationToken ct)
    {
        return await _db.ContentProducts
            .Where(cp => cp.ContentId == contentId)
            .OrderBy(cp => cp.SortOrder)
            .Select(cp => new ContentProductDto(
                cp.StoreProductId,
                cp.StoreProduct.ProductName,
                cp.StoreProduct.Price,
                cp.StoreProduct.ImageUrl,
                cp.SortOrder))
            .ToListAsync(ct);
    }

    private async Task<Domain.Entities.Content> Find(long retailerId, long contentId, CancellationToken ct) =>
        await _db.Contents.FirstOrDefaultAsync(c => c.RetailerId == retailerId && c.ContentId == contentId, ct)
            ?? throw AppException.NotFound("Content");

    private static string Hash(string input)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(input)))[..16].ToLowerInvariant();

    private async Task<ContentDto> MapAsync(Domain.Entities.Content c, List<ContentProductDto>? products, CancellationToken ct)
    {
        var expiry = TimeSpan.FromHours(6);
        string? thumbUrl = null, previewUrl = null;

        if (c.ContentType == ContentType.Image)
        {
            Dictionary<string, string>? variants = null;
            if (!string.IsNullOrWhiteSpace(c.MediaVariants))
                try { variants = JsonSerializer.Deserialize<Dictionary<string, string>>(c.MediaVariants); } catch { /* ignore */ }

            var thumbKey = c.ThumbnailKey ?? variants?.GetValueOrDefault("thumb");
            var previewKey = variants?.GetValueOrDefault("md") ?? variants?.GetValueOrDefault("lg")
                ?? variants?.GetValueOrDefault("sm") ?? c.MasterKey;

            if (thumbKey is not null) thumbUrl = await _storage.CreateDownloadUrlAsync(thumbKey, expiry, ct);
            if (previewKey is not null) previewUrl = await _storage.CreateDownloadUrlAsync(previewKey, expiry, ct);
        }
        else if (c.ContentType == ContentType.Video)
        {
            if (c.ThumbnailKey is not null) thumbUrl = await _storage.CreateDownloadUrlAsync(c.ThumbnailKey, expiry, ct);
            if (c.MasterKey is not null) previewUrl = await _storage.CreateDownloadUrlAsync(c.MasterKey, expiry, ct);
        }

        return new ContentDto(
            c.ContentId, c.ContentType.ToString(), c.Name, c.MasterKey, c.MediaVariants,
            c.ThumbnailKey, c.DurationSeconds, c.Version, c.IsActive, products, thumbUrl, previewUrl);
    }
}
