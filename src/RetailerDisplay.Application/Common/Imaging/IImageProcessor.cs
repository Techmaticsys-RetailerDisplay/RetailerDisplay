namespace RetailerDisplay.Application.Common.Imaging;

/// <summary>A generated image variant (WebP) at a target width.</summary>
public record ImageRendition(string Name, int Width, byte[] Data)
{
    public string ContentType => "image/webp";
}

/// <summary>
/// Produces responsive WebP renditions from an uploaded image.
/// Implementation is swappable (currently ImageSharp) via this interface.
/// </summary>
public interface IImageProcessor
{
    /// <summary>
    /// Reads the source image and returns WebP renditions: sm (720w), md (1080w),
    /// lg (2160w), and thumb (320w). Renditions are never upscaled past the source width.
    /// </summary>
    Task<IReadOnlyList<ImageRendition>> CreateRenditionsAsync(Stream source, CancellationToken ct = default);
}
