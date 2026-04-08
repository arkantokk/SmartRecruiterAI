using Microsoft.AspNetCore.Identity;
using SmartRecruiter.Application.DTOs;
using SmartRecruiter.Application.Interfaces;
using SmartRecruiter.Domain.DTOs;
using SmartRecruiter.Domain.Entities;
using SmartRecruiter.Domain.Interfaces;

namespace SmartRecruiter.Infrastructure.Services;

public class IdentityAuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly ITokensRepository _repository;

    public IdentityAuthService(UserManager<IdentityUser> userManager, ITokenService tokenService, ITokensRepository tokensRepository)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _repository = tokensRepository;
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
            var refreshTokenString = _tokenService.GenerateRefreshToken(); // generating string for refreshToken
            var refreshToken = new RefreshToken(refreshTokenString, DateTime.UtcNow.AddDays(30), user.Id); //creating new object of refresh token
            await _repository.AddAsync(refreshToken);
            return new AuthResult(true, new List<string>(), tokenString,refreshTokenString);
        }

        var errorMessages = result.Errors.Select(e => e.Description).ToList();
        var errorToken = String.Empty;
        
        return new AuthResult(result.Succeeded, errorMessages, errorToken,  string.Empty);
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) throw new UnauthorizedAccessException("Invalid email or password.");
        var result = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!result) throw new UnauthorizedAccessException("Invalid email or password.");
        var token = new TokenUserDto(user.Id, user.Email);
        var tokenString = _tokenService.GenerateToken(token);
        var refreshTokenString = _tokenService.GenerateRefreshToken(); // generating string for refreshToken
        var refreshToken = new RefreshToken(refreshTokenString, DateTime.UtcNow.AddDays(30), user.Id); //creating new object of refresh token
        await _repository.AddAsync(refreshToken);
        return new AuthResult(true, new List<string>(), tokenString, refreshTokenString);
    }

    public async Task<bool> RevokeAsync(string refreshToken)
    {
        var token = await _repository.GetByTokenAsync(refreshToken);
        if (token == null)
        {
            return false;
        }
        token.Revoke();
        await _repository.UpdateAsync(token);
        return true;
    }
}