namespace RetailerDisplay.Application.Common;

/// <summary>Upload limits, enforced on both the client and the API.</summary>
public static class MediaLimits
{
    public const long MaxImageBytes = 2 * 1024 * 1024;    // 2 MB
    public const long MaxVideoBytes = 20 * 1024 * 1024;   // 20 MB
    public const int MaxVideoSeconds = 60;                // 1 minute

    public static readonly string[] AllowedImageTypes = { "image/jpeg", "image/png", "image/webp" };
    public static readonly string[] AllowedVideoTypes = { "video/mp4" };

    public const int DefaultImageDurationSeconds = 10;
}
