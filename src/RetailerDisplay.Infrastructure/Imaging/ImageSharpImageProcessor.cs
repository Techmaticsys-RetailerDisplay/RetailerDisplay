using RetailerDisplay.Application.Common.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace RetailerDisplay.Infrastructure.Imaging;

/// <summary>
/// Generates WebP renditions with ImageSharp. Isolated behind IImageProcessor so the
/// underlying library can be swapped (see the pending image-library decision).
/// </summary>
public class ImageSharpImageProcessor : IImageProcessor
{
    private static readonly (string Name, int Width)[] Targets =
    {
        ("thumb", 320),
        ("sm", 720),
        ("md", 1080),
        ("lg", 2160)
    };

    public async Task<IReadOnlyList<ImageRendition>> CreateRenditionsAsync(Stream source, CancellationToken ct = default)
    {
        using var image = await Image.LoadAsync(source, ct);
        var encoder = new WebpEncoder { Quality = 80 };
        var renditions = new List<ImageRendition>(Targets.Length);

        foreach (var (name, width) in Targets)
        {
            // Never upscale past the source width.
            var effectiveWidth = Math.Min(width, image.Width);

            using var clone = image.Clone(ctx => ctx.Resize(new ResizeOptions
            {
                Size = new Size(effectiveWidth, 0),
                Mode = ResizeMode.Max
            }));

            using var ms = new MemoryStream();
            await clone.SaveAsync(ms, encoder, ct);
            renditions.Add(new ImageRendition(name, effectiveWidth, ms.ToArray()));
        }

        return renditions;
    }
}
