using Microsoft.EntityFrameworkCore;
using RetailerDisplay.Application.Common;
using RetailerDisplay.Application.Common.Security;
using RetailerDisplay.Domain.Entities;

namespace RetailerDisplay.Application.Auth;

public class AuthService : IAuthService
{
    private readonly IApplicationDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _tokens;

    public AuthService(IApplicationDbContext db, IPasswordHasher passwordHasher, IJwtTokenService tokens)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _tokens = tokens;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var retailer = await _db.Retailers.FirstOrDefaultAsync(r => r.Email == email, ct);

        if (retailer is null || !retailer.IsActive || !_passwordHasher.Verify(request.Password, retailer.PasswordHash))
            throw AppException.Unauthorized("Invalid email or password.");

        return await IssueAsync(retailer, ct);
    }

    public async Task<AuthResponse> RefreshAsync(RefreshRequest request, CancellationToken ct = default)
    {
        var hash = _tokens.HashRefreshToken(request.RefreshToken);
        var stored = await _db.RefreshTokens
            .Include(t => t.Retailer)
            .FirstOrDefaultAsync(t => t.TokenHash == hash, ct);

        if (stored is null || stored.RevokedAt is not null || stored.ExpiresAt <= DateTime.UtcNow)
            throw AppException.Unauthorized("Invalid or expired refresh token.");

        // Rotate: revoke the presented token, then issue a fresh pair.
        stored.RevokedAt = DateTime.UtcNow;
        var response = await IssueAsync(stored.Retailer, ct);
        return response;
    }

    public async Task LogoutAsync(RefreshRequest request, CancellationToken ct = default)
    {
        var hash = _tokens.HashRefreshToken(request.RefreshToken);
        var stored = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hash, ct);
        if (stored is not null && stored.RevokedAt is null)
        {
            stored.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }
    }

    public async Task<RetailerProfile> GetProfileAsync(long retailerId, CancellationToken ct = default)
    {
        var retailer = await _db.Retailers.FirstOrDefaultAsync(r => r.RetailerId == retailerId, ct)
            ?? throw AppException.NotFound("Retailer");
        return ToProfile(retailer);
    }

    public async Task<RetailerProfile> UpdateProfileAsync(long retailerId, UpdateProfileRequest r, CancellationToken ct = default)
    {
        var retailer = await _db.Retailers.FirstOrDefaultAsync(x => x.RetailerId == retailerId, ct)
            ?? throw AppException.NotFound("Retailer");

        retailer.BusinessName = r.BusinessName.Trim();
        retailer.ContactName = r.ContactName?.Trim();
        retailer.Phone = r.Phone?.Trim();
        retailer.AddressLine1 = r.AddressLine1?.Trim();
        retailer.AddressLine2 = r.AddressLine2?.Trim();
        retailer.City = r.City?.Trim();
        retailer.State = r.State?.Trim();
        retailer.PostalCode = r.PostalCode?.Trim();
        retailer.Country = r.Country?.Trim();

        // Required fields for a "complete" profile.
        retailer.ProfileCompleted =
            !string.IsNullOrWhiteSpace(retailer.BusinessName) &&
            !string.IsNullOrWhiteSpace(retailer.ContactName) &&
            !string.IsNullOrWhiteSpace(retailer.Phone) &&
            !string.IsNullOrWhiteSpace(retailer.AddressLine1) &&
            !string.IsNullOrWhiteSpace(retailer.City) &&
            !string.IsNullOrWhiteSpace(retailer.State) &&
            !string.IsNullOrWhiteSpace(retailer.PostalCode) &&
            !string.IsNullOrWhiteSpace(retailer.Country);

        retailer.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return ToProfile(retailer);
    }

    private async Task<AuthResponse> IssueAsync(Retailer retailer, CancellationToken ct)
    {
        var (accessToken, accessExpires) = _tokens.CreateAccessToken(retailer);
        var (rawRefresh, refreshHash, refreshExpires) = _tokens.CreateRefreshToken();

        _db.RefreshTokens.Add(new RefreshToken
        {
            RetailerId = retailer.RetailerId,
            TokenHash = refreshHash,
            ExpiresAt = refreshExpires,
            CreatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync(ct);

        return new AuthResponse(accessToken, accessExpires, rawRefresh, ToProfile(retailer));
    }

    private static RetailerProfile ToProfile(Retailer r) =>
        new(r.RetailerId, r.Email, r.BusinessName, r.ContactName, r.Phone,
            r.AddressLine1, r.AddressLine2, r.City, r.State, r.PostalCode, r.Country, r.ProfileCompleted);
}
