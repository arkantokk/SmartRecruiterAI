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
    private readonly IConfiguration _configuration;
    public IntegrationsController(IGmailAuthService authService, IConfiguration configuration)
    {
        _authService = authService;
        _configuration = configuration;
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
        var frontendUrl = _configuration["FrontendUrl"]?.TrimEnd('/');
        if (state == null || code == null)
        {
            return Redirect($"{frontendUrl}/candidates?gmail=failure");
        }

        await _authService.HandleCallbackAsync(code, state);
        return Redirect($"{frontendUrl}/candidates?gmail=success");
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