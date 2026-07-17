using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailerDisplay.Application.Common;
using RetailerDisplay.Application.Common.Storage;
using RetailerDisplay.Application.Media;

namespace RetailerDisplay.Api.Controllers;

[ApiController]
[Route("api/v1/media")]
public class MediaController : ControllerBase
{
    private readonly IMediaService _media;
    private readonly IMediaStorage _storage;
    private readonly ICurrentUser _currentUser;

    public MediaController(IMediaService media, IMediaStorage storage, ICurrentUser currentUser)
    {
        _media = media;
        _storage = storage;
        _currentUser = currentUser;
    }

    /// <summary>Returns a presigned URL for uploading directly to storage (S3 mode).</summary>
    [Authorize]
    [HttpPost("upload-url")]
    public async Task<ActionResult<UploadUrlResponse>> CreateUploadUrl(UploadUrlRequest request, CancellationToken ct)
        => Ok(await _media.CreateUploadUrlAsync(_currentUser.RequireRetailerId(), request, ct));

    /// <summary>Direct (proxied) upload of an image/video. Returns the stored object key.</summary>
    [Authorize]
    [HttpPost("upload")]
    [RequestSizeLimit(25 * 1024 * 1024)]
    public async Task<ActionResult<UploadedMedia>> Upload([FromForm] IFormFile file, [FromForm] string kind, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            throw new AppException("A file is required.");

        var uploadKind = string.Equals(kind, "video", StringComparison.OrdinalIgnoreCase)
            ? MediaUploadKind.Video : MediaUploadKind.Image;

        await using var stream = file.OpenReadStream();
        var result = await _media.UploadAsync(
            _currentUser.RequireRetailerId(), uploadKind, file.FileName, file.ContentType, file.Length, stream, ct);
        return Ok(result);
    }

    /// <summary>Serves a stored file (used by local storage; S3 serves via presigned URLs).</summary>
    [AllowAnonymous]
    [HttpGet("file/{**key}")]
    public async Task<IActionResult> GetFile(string key, CancellationToken ct)
    {
        try
        {
            var stream = await _storage.OpenReadAsync(key, ct);
            return File(stream, ContentTypeFor(key));
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
    }

    private static string ContentTypeFor(string key) => Path.GetExtension(key).ToLowerInvariant() switch
    {
        ".webp" => "image/webp",
        ".png" => "image/png",
        ".jpg" or ".jpeg" => "image/jpeg",
        ".mp4" => "video/mp4",
        _ => "application/octet-stream"
    };
}
