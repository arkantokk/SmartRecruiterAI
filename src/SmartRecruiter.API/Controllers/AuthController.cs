using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartRecruiter.Application.DTOs;
using SmartRecruiter.Application.Interfaces;

namespace SmartRecruiter.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        if (result.Succeeded)
        {
            SetRefreshTokenCookie(result.RefreshToken);
            return Ok(new {token = result.Token});
        }
        else
        {
            return BadRequest(result.Errors);
        }
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        var token = await _authService.LoginAsync(request);
        if (token != null)
        {
            SetRefreshTokenCookie(token.RefreshToken);
            return Ok(new {token = token.Token});
        }
        else
        {
            return BadRequest();
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (!string.IsNullOrEmpty(refreshToken))
        {
            await _authService.RevokeAsync(refreshToken);
        }
        Response.Cookies.Delete("refreshToken");

        return Ok(new { message = "Logged out successfully" });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshTokenAsync()
    {
        var token = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(token) )
        {
            return Unauthorized();
        }
        var result = await _authService.RefreshAsync(token);
        if (!result.IsAuth) return Unauthorized();
        SetRefreshTokenCookie(result.refreshToken);
        return Ok(new {token = result.Token});

    }
    
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var user = User.FindFirstValue(ClaimTypes.Name);
        return Ok(new { user = user });
    }

    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLoginAsync([FromBody] GoogleLoginRequest request)
    {
        var token = request.Token;
        var result = await _authService.GoogleLoginAsync(token);
        if (!result.Succeeded) return BadRequest();
        SetRefreshTokenCookie(result.RefreshToken);
        return Ok(new { token = result.Token });
    }
    
    private void SetRefreshTokenCookie(string refreshToken)
    {
        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddDays(30)
        };
        
        Response.Cookies.Append("refreshToken", refreshToken, options);
    }

    
}