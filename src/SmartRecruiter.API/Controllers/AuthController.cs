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
            return Ok();
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
            return Ok(token);
        }
        else
        {
            return BadRequest();
        }
    }
}