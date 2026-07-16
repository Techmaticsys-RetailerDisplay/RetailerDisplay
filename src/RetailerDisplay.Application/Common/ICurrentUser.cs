namespace RetailerDisplay.Application.Common;

/// <summary>The authenticated retailer for the current request. Implemented in the API layer.</summary>
public interface ICurrentUser
{
    /// <summary>RetailerId from the JWT, or null when unauthenticated.</summary>
    long? RetailerId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }

    /// <summary>RetailerId, throwing if the request is not authenticated.</summary>
    long RequireRetailerId();
}
