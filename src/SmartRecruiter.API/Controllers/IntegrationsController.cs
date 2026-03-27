using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartRecruiter.Application.Interfaces;

namespace SmartRecruiter.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IntegrationsController : ControllerBase
{
    private readonly IGmailAuthService _authService;

    public IntegrationsController(IGmailAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet("google/connect")]
    [Authorize]
    public IActionResult Connect()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Forbid();
        }

        var responseUrl = _authService.GetAuthorizationUrl(userId);
        return Ok(new { Url = responseUrl });
    }

    [HttpGet("google/callback")]
    public async Task<IActionResult> HandleCallBack(string code, string state)
    {
        if (state == null || code == null)
        {
            return Redirect("http://localhost:5173/candidates?gmail=failure");
        }

        await _authService.HandleCallbackAsync(code, state);
        return Redirect("http://localhost:5173/candidates?gmail=success");
    }

    [HttpGet("google/status")]
    [Authorize]
    public async Task<IActionResult> CheckStatus()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Forbid();
        }

        var status = await _authService.IntegrationStatus(userId);
        
        return Ok(status);
    }
}