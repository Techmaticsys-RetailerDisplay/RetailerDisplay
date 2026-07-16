using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using RetailerDisplay.Application.Common.Storage;

namespace RetailerDisplay.Infrastructure.Storage;

/// <summary>S3-backed media storage. Works against AWS S3 or MinIO (via ServiceUrl + ForcePathStyle).</summary>
public class S3MediaStorage : IMediaStorage
{
    private readonly IAmazonS3 _s3;
    private readonly S3Options _options;

    public S3MediaStorage(IAmazonS3 s3, IOptions<S3Options> options)
    {
        _s3 = s3;
        _options = options.Value;
    }

    public Task<PresignedUpload> CreateUploadUrlAsync(string key, string contentType, TimeSpan expiry, CancellationToken ct = default)
    {
        var expiresAt = DateTime.UtcNow.Add(expiry);
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _options.Bucket,
            Key = key,
            Verb = HttpVerb.PUT,
            ContentType = contentType,
            Expires = expiresAt
        };
        var url = _s3.GetPreSignedURL(request);
        return Task.FromResult(new PresignedUpload(url, key, expiresAt));
    }

    public Task<string> CreateDownloadUrlAsync(string key, TimeSpan expiry, CancellationToken ct = default)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _options.Bucket,
            Key = key,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.Add(expiry)
        };
        return Task.FromResult(_s3.GetPreSignedURL(request));
    }

    public async Task SaveAsync(string key, Stream content, string contentType, CancellationToken ct = default)
    {
        await _s3.PutObjectAsync(new PutObjectRequest
        {
            BucketName = _options.Bucket,
            Key = key,
            InputStream = content,
            ContentType = contentType,
            AutoCloseStream = false
        }, ct);
    }

    public async Task<Stream> OpenReadAsync(string key, CancellationToken ct = default)
    {
        var response = await _s3.GetObjectAsync(_options.Bucket, key, ct);
        var memory = new MemoryStream();
        await response.ResponseStream.CopyToAsync(memory, ct);
        memory.Position = 0;
        return memory;
    }

    public Task DeleteAsync(string key, CancellationToken ct = default)
        => _s3.DeleteObjectAsync(_options.Bucket, key, ct);
}
