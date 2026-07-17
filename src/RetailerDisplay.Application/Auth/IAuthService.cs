namespace RetailerDisplay.Application.Auth;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<AuthResponse> RefreshAsync(RefreshRequest request, CancellationToken ct = default);
    Task LogoutAsync(RefreshRequest request, CancellationToken ct = default);
    Task<RetailerProfile> GetProfileAsync(long retailerId, CancellationToken ct = default);
    Task<RetailerProfile> UpdateProfileAsync(long retailerId, UpdateProfileRequest request, CancellationToken ct = default);
}
