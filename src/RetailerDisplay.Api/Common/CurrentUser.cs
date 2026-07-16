using System.Security.Claims;
using RetailerDisplay.Application.Common;

namespace RetailerDisplay.Api.Common;

/// <summary>Reads the authenticated retailer from the current request's JWT claims.</summary>
public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUser(IHttpContextAccessor accessor) => _accessor = accessor;

    private ClaimsPrincipal? User => _accessor.HttpContext?.User;

    public long? RetailerId =>
        long.TryParse(User?.FindFirst("retailerId")?.Value, out var id) ? id : null;

    public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value
        ?? User?.FindFirst("email")?.Value;

    public bool IsAuthenticated => RetailerId.HasValue;

    public long RequireRetailerId() =>
        RetailerId ?? throw AppException.Unauthorized();
}
