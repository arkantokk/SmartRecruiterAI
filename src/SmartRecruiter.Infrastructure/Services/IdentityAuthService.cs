using Microsoft.AspNetCore.Identity;
using SmartRecruiter.Application.DTOs;
using SmartRecruiter.Application.Interfaces;
using SmartRecruiter.Domain.DTOs;

namespace SmartRecruiter.Infrastructure.Services;

public class IdentityAuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ITokenService _tokenService;


    public IdentityAuthService(UserManager<IdentityUser> userManager, ITokenService tokenService)
    {
        _tokenService = tokenService;
        _userManager = userManager;
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        var user = new IdentityUser { UserName = request.Email, Email = request.Email };
        var result = await _userManager.CreateAsync(user, request.Password);
        if (result.Succeeded)
        {
            // generating token
            var token = new TokenUserDto(user.Id, user.Email);
            var tokenString = _tokenService.GenerateToken(token);
            
            return new AuthResult(true, new List<string>(), tokenString);
        }

        var errorMessages = result.Errors.Select(e => e.Description).ToList();
        var errorToken = String.Empty;
        return new AuthResult(result.Succeeded, errorMessages, errorToken );
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) throw new UnauthorizedAccessException("Invalid email or password.");
        var result = await _userManager.CheckPasswordAsync(user, request.Password);
        if (result)
        {
            var token = new TokenUserDto(user.Id, user.Email);
            var tokenString = _tokenService.GenerateToken(token);
            return new AuthResult(true, new List<string>(), tokenString);
        }
        else
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }
    }
}