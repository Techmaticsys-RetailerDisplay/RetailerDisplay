namespace RetailerDisplay.Application.Media;

public interface IMediaService
{
    /// <summary>Validates limits/type and returns a presigned URL for direct upload to storage.</summary>
    Task<UploadUrlResponse> CreateUploadUrlAsync(long retailerId, UploadUrlRequest request, CancellationToken ct = default);
}
