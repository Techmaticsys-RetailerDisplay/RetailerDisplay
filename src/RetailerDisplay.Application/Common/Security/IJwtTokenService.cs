using RetailerDisplay.Domain.Entities;

namespace RetailerDisplay.Application.Common.Security;

/// <summary>Issues JWT access tokens and opaque refresh tokens for retailer sessions.</summary>
public interface IJwtTokenService
{
    /// <summary>Creates a signed access token for the retailer.</summary>
    (string token, DateTime expiresAt) CreateAccessToken(Retailer retailer);

    /// <summary>Generates a new random refresh token (raw value) plus its stored hash and expiry.</summary>
    (string rawToken, string tokenHash, DateTime expiresAt) CreateRefreshToken();

    /// <summary>Hashes a raw refresh token for lookup/comparison against stored hashes.</summary>
    string HashRefreshToken(string rawToken);
}
