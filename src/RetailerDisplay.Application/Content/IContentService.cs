using RetailerDisplay.Domain.Enums;

namespace RetailerDisplay.Application.Content;

public interface IContentService
{
    Task<IReadOnlyList<ContentDto>> ListAsync(long retailerId, ContentType? type, CancellationToken ct = default);
    Task<ContentDto> GetAsync(long retailerId, long contentId, CancellationToken ct = default);
    Task<ContentDto> CreateAsync(long retailerId, CreateContentRequest request, CancellationToken ct = default);
    Task<ContentDto> UpdateAsync(long retailerId, long contentId, UpdateContentRequest request, CancellationToken ct = default);
    Task SetProductsAsync(long retailerId, long contentId, SetContentProductsRequest request, CancellationToken ct = default);
    Task DeleteAsync(long retailerId, long contentId, CancellationToken ct = default);
}
