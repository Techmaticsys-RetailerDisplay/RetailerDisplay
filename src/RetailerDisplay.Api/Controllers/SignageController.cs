using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using RetailerDisplay.Application.Signage;

namespace RetailerDisplay.Api.Controllers;

/// <summary>
/// Device-facing signage feed in the legacy GetSignageItemResult format
/// (PascalCase, GetSignageItemResult wrapper, nulls omitted). Auth is by DeviceId (device key).
/// </summary>
[ApiController]
[Route("api/v1/signage")]
public class SignageController : ControllerBase
{
    // Match legacy Newtonsoft output: PascalCase property names, null values omitted.
    private static readonly JsonSerializerOptions LegacyJson = new()
    {
        PropertyNamingPolicy = null,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly ISignageService _signage;

    public SignageController(ISignageService signage) => _signage = signage;

    [HttpPost("GetSignageItemResult")]
    public async Task<IActionResult> GetSignageItemResult([FromBody] SignageRequest request, CancellationToken ct)
    {
        var result = await _signage.GetSignageItemResultAsync(request ?? new SignageRequest(), ct);
        var json = JsonSerializer.Serialize(result, LegacyJson);
        return Content(json, "application/json");
    }
}
