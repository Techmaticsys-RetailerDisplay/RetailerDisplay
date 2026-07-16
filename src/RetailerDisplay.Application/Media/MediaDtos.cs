namespace RetailerDisplay.Application.Media;

public enum MediaUploadKind
{
    Image = 1,
    Video = 2
}

public record UploadUrlRequest(MediaUploadKind Kind, string FileName, string ContentType, long SizeBytes);

public record UploadUrlResponse(string Url, string Key, DateTime ExpiresAt);
