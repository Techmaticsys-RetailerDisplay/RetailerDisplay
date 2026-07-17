namespace RetailerDisplay.Application.Auth;

public record LoginRequest(string Email, string Password);

public record RefreshRequest(string RefreshToken);

public record AuthResponse(
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    string RefreshToken,
    RetailerProfile Retailer);

public record RetailerProfile(
    long RetailerId,
    string Email,
    string BusinessName,
    string? ContactName,
    string? Phone,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? State,
    string? PostalCode,
    string? Country,
    bool ProfileCompleted);

public record UpdateProfileRequest(
    string BusinessName,
    string? ContactName,
    string? Phone,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? State,
    string? PostalCode,
    string? Country);
