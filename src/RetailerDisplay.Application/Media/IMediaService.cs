namespace RetailerDisplay.Application.Media;

public record UploadedMedia(string Key, string ContentType, long SizeBytes);

public interface IMediaService
{
    /// <summary>Validates limits/type and returns a presigned URL for direct upload to storage.</summary>
    Task<UploadUrlResponse> CreateUploadUrlAsync(long retailerId, UploadUrlRequest request, CancellationToken ct = default);

    /// <summary>Direct (proxied) upload: validates, stores the bytes, and returns the object key.</summary>
    Task<UploadedMedia> UploadAsync(long retailerId, MediaUploadKind kind, string fileName, string contentType, long sizeBytes, Stream content, CancellationToken ct = default);
}
