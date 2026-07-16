using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailerDisplay.Application.Common;
using RetailerDisplay.Application.Media;

namespace RetailerDisplay.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/media")]
public class MediaController : ControllerBase
{
    private readonly IMediaService _media;
    private readonly ICurrentUser _currentUser;

    public MediaController(IMediaService media, ICurrentUser currentUser)
    {
        _media = media;
        _currentUser = currentUser;
    }

    /// <summary>Returns a presigned URL for uploading an image/video directly to storage.</summary>
    [HttpPost("upload-url")]
    public async Task<ActionResult<UploadUrlResponse>> CreateUploadUrl(UploadUrlRequest request, CancellationToken ct)
        => Ok(await _media.CreateUploadUrlAsync(_currentUser.RequireRetailerId(), request, ct));
}
