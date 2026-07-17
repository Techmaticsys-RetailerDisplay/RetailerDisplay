namespace RetailerDisplay.Application.Signage;

/// <summary>Serves the device playlist in the legacy GetSignageItemResult format.</summary>
public interface ISignageService
{
    Task<SignageResultDto> GetSignageItemResultAsync(SignageRequest request, CancellationToken ct = default);
}
