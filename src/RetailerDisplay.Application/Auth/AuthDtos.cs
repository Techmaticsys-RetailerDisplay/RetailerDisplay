namespace RetailerDisplay.Application.Auth;

public record LoginRequest(string Email, string Password);

public record RegisterRequest(string Email, string Password, string BusinessName, string? ContactName, string? Phone);

public record RefreshRequest(string RefreshToken);

public record AuthResponse(
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    string RefreshToken,
    RetailerProfile Retailer);

public record RetailerProfile(long RetailerId, string Email, string BusinessName, string? ContactName, string? Phone);
