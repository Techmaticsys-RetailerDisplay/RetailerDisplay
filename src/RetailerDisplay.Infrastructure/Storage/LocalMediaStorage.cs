using Microsoft.Extensions.Options;
using RetailerDisplay.Application.Common.Storage;

namespace RetailerDisplay.Infrastructure.Storage;

/// <summary>
/// Filesystem-backed media storage for local development (no S3/MinIO required).
/// Files are served back via the API's GET /media/file/{**key} endpoint.
/// </summary>
public class LocalMediaStorage : IMediaStorage
{
    private readonly string _root;
    private readonly string _publicBaseUrl;

    public LocalMediaStorage(IOptions<MediaOptions> options)
    {
        var o = options.Value;
        _root = Path.IsPathRooted(o.LocalRoot) ? o.LocalRoot : Path.Combine(Directory.GetCurrentDirectory(), o.LocalRoot);
        _publicBaseUrl = o.PublicBaseUrl.TrimEnd('/');
        Directory.CreateDirectory(_root);
    }

    public Task<PresignedUpload> CreateUploadUrlAsync(string key, string contentType, TimeSpan expiry, CancellationToken ct = default)
        => throw new NotSupportedException("Local storage uses direct upload via POST /media/upload.");

    public Task<string> CreateDownloadUrlAsync(string key, TimeSpan expiry, CancellationToken ct = default)
    {
        // Already an absolute URL (e.g. an externally-hosted product image) — return unchanged.
        if (IsAbsoluteUrl(key)) return Task.FromResult(key);
        return Task.FromResult($"{_publicBaseUrl}/api/v1/media/file/{key}");
    }

    private static bool IsAbsoluteUrl(string key) =>
        key.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
        key.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

    public async Task SaveAsync(string key, Stream content, string contentType, CancellationToken ct = default)
    {
        var path = PathFor(key);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        content.Position = content.CanSeek ? 0 : content.Position;
        await content.CopyToAsync(fs, ct);
    }

    public Task<Stream> OpenReadAsync(string key, CancellationToken ct = default)
    {
        var path = PathFor(key);
        if (!File.Exists(path)) throw new FileNotFoundException("Media object not found.", key);
        Stream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        return Task.FromResult(fs);
    }

    public Task DeleteAsync(string key, CancellationToken ct = default)
    {
        var path = PathFor(key);
        if (File.Exists(path)) File.Delete(path);
        return Task.CompletedTask;
    }

    private string PathFor(string key)
    {
        // Prevent path traversal; keys are app-generated but be safe.
        var safe = key.Replace("..", "").Replace('\\', '/').TrimStart('/');
        return Path.Combine(_root, safe.Replace('/', Path.DirectorySeparatorChar));
    }
}
