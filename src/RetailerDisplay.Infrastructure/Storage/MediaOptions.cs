namespace RetailerDisplay.Infrastructure.Storage;

/// <summary>Bound from the "Media" configuration section — selects the storage provider.</summary>
public class MediaOptions
{
    /// <summary>"S3" (default) or "Local" (dev: files on disk, no S3/MinIO needed).</summary>
    public string Provider { get; set; } = "S3";
    /// <summary>Folder for local storage (relative to the app's working directory).</summary>
    public string LocalRoot { get; set; } = "media-store";
    /// <summary>Base URL the API is reachable at, used to build local file URLs.</summary>
    public string PublicBaseUrl { get; set; } = "http://localhost:5080";
}
