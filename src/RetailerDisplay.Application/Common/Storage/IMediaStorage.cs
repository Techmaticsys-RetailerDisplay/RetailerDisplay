namespace RetailerDisplay.Application.Common.Storage;

public record PresignedUpload(string Url, string Key, DateTime ExpiresAt);

/// <summary>
/// Abstraction over object storage (S3 in prod, MinIO in dev). The API hands out
/// presigned URLs so browsers/devices transfer bytes directly, and reads/writes
/// derived files (renditions) server-side.
/// </summary>
public interface IMediaStorage
{
    /// <summary>Creates a presigned PUT URL the client uploads directly to.</summary>
    Task<PresignedUpload> CreateUploadUrlAsync(string key, string contentType, TimeSpan expiry, CancellationToken ct = default);

    /// <summary>Creates a presigned GET URL for reading a private object.</summary>
    Task<string> CreateDownloadUrlAsync(string key, TimeSpan expiry, CancellationToken ct = default);

    /// <summary>Writes bytes to storage (used for generated renditions/thumbnails).</summary>
    Task SaveAsync(string key, Stream content, string contentType, CancellationToken ct = default);

    /// <summary>Opens an object for reading (used to generate renditions from an upload).</summary>
    Task<Stream> OpenReadAsync(string key, CancellationToken ct = default);

    Task DeleteAsync(string key, CancellationToken ct = default);
}
