namespace RetailerDisplay.Application.Common;

/// <summary>
/// A business/validation error the API surfaces to the client with a specific status code.
/// Caught by the API's exception-handling middleware and rendered as RFC 7807.
/// </summary>
public class AppException : Exception
{
    public int StatusCode { get; }

    public AppException(string message, int statusCode = 400) : base(message)
    {
        StatusCode = statusCode;
    }

    public static AppException NotFound(string what) => new($"{what} was not found.", 404);
    public static AppException Unauthorized(string message = "Unauthorized.") => new(message, 401);
    public static AppException Conflict(string message) => new(message, 409);
}
