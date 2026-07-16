using RetailerDisplay.Application.Common;
using RetailerDisplay.Application.Common.Storage;

namespace RetailerDisplay.Application.Media;

public class MediaService : IMediaService
{
    private readonly IMediaStorage _storage;

    public MediaService(IMediaStorage storage) => _storage = storage;

    public async Task<UploadUrlResponse> CreateUploadUrlAsync(long retailerId, UploadUrlRequest request, CancellationToken ct = default)
    {
        var contentType = request.ContentType.Trim().ToLowerInvariant();

        string folder;
        if (request.Kind == MediaUploadKind.Image)
        {
            if (!MediaLimits.AllowedImageTypes.Contains(contentType))
                throw new AppException("Unsupported image type. Allowed: JPEG, PNG, WebP.");
            if (request.SizeBytes > MediaLimits.MaxImageBytes)
                throw new AppException("Image exceeds the 2 MB limit.");
            folder = "images";
        }
        else
        {
            if (!MediaLimits.AllowedVideoTypes.Contains(contentType))
                throw new AppException("Unsupported video type. Allowed: MP4.");
            if (request.SizeBytes > MediaLimits.MaxVideoBytes)
                throw new AppException("Video exceeds the 20 MB limit.");
            folder = "videos";
        }

        var safeName = SanitizeFileName(request.FileName);
        var key = $"{retailerId}/{folder}/{Guid.NewGuid():N}/{safeName}";

        var presigned = await _storage.CreateUploadUrlAsync(key, contentType, TimeSpan.FromMinutes(15), ct);
        return new UploadUrlResponse(presigned.Url, presigned.Key, presigned.ExpiresAt);
    }

    private static string SanitizeFileName(string fileName)
    {
        var name = Path.GetFileName(fileName);
        foreach (var c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');
        return string.IsNullOrWhiteSpace(name) ? "upload" : name;
    }
}
