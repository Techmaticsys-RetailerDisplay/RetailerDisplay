using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailerDisplay.Application.Auth;
using RetailerDisplay.Application.Common;

namespace RetailerDisplay.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly ICurrentUser _currentUser;

    public AuthController(IAuthService auth, ICurrentUser currentUser)
    {
        _auth = auth;
        _currentUser = currentUser;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken ct)
        => Ok(await _auth.LoginAsync(request, ct));

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(RefreshRequest request, CancellationToken ct)
        => Ok(await _auth.RefreshAsync(request, ct));

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshRequest request, CancellationToken ct)
    {
        await _auth.LogoutAsync(request, ct);
        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<RetailerProfile>> Me(CancellationToken ct)
        => Ok(await _auth.GetProfileAsync(_currentUser.RequireRetailerId(), ct));

    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<RetailerProfile>> GetProfile(CancellationToken ct)
        => Ok(await _auth.GetProfileAsync(_currentUser.RequireRetailerId(), ct));

    [Authorize]
    [HttpPut("profile")]
    public async Task<ActionResult<RetailerProfile>> UpdateProfile(UpdateProfileRequest request, CancellationToken ct)
        => Ok(await _auth.UpdateProfileAsync(_currentUser.RequireRetailerId(), request, ct));
}
