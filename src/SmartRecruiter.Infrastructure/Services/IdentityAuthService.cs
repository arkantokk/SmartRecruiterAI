using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
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
    private readonly IConfiguration _configuration;

    public IdentityAuthService(UserManager<IdentityUser> userManager, ITokenService tokenService,
        ITokensRepository tokensRepository, IConfiguration configuration)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _repository = tokensRepository;
        _configuration = configuration;
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        var user = new IdentityUser { UserName = request.Email, Email = request.Email };
        var result = await _userManager.CreateAsync(user, request.Password);
        if (result.Succeeded)
        {
            var tokens = await GenerateAndSaveTokenAsync(user);
            return new AuthResult(true, new List<string>(), tokens.TokenString, tokens.RefreshTokenString);
        }

        var errorMessages = result.Errors.Select(e => e.Description).ToList();
        var errorToken = String.Empty;

        return new AuthResult(result.Succeeded, errorMessages, errorToken, string.Empty);
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) throw new UnauthorizedAccessException("Invalid email or password.");
        var result = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!result) throw new UnauthorizedAccessException("Invalid email or password.");
        var tokens = await GenerateAndSaveTokenAsync(user);
        return new AuthResult(true, new List<string>(), tokens.TokenString, tokens.RefreshTokenString);
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

    public async Task<TokensResult> RefreshAsync(string refreshToken)
    {
        var token = await _repository.GetByTokenAsync(refreshToken);
        if (token == null || token.IsRevoked == true || token.ExpiryDate < DateTime.UtcNow)
        {
            return new TokensResult(false, string.Empty, string.Empty);
        }

        var user = await _userManager.FindByIdAsync(token.UserId);
        // generating token
        var tokens = await GenerateAndSaveTokenAsync(user);
        await RevokeAsync(refreshToken);
        return new TokensResult(true, tokens.TokenString, tokens.RefreshTokenString);
    }

    public async Task<AuthResult> GoogleLoginAsync(string googleToken)
    {
        var clientId = _configuration["GoogleAuth:ClientId"];
        if (string.IsNullOrEmpty(clientId))
        {
            throw new Exception("Add env of google Google:ClientId");
        }
        var validation = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = new List<string> { clientId }
        };
        var payload = await GoogleJsonWebSignature.ValidateAsync(googleToken, validation);
        var identityUser = await _userManager.FindByEmailAsync(payload.Email);
        if (identityUser == null)
        {
            var user = new IdentityUser { UserName = payload.Email, Email = payload.Email };
            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                var tokens = await GenerateAndSaveTokenAsync(user); // generating token
                return new AuthResult(true, new List<string>(), tokens.TokenString, tokens.RefreshTokenString);
            }

            var errorMessages = result.Errors.Select(e => e.Description).ToList();
            var errorToken = String.Empty;

            return new AuthResult(result.Succeeded, errorMessages, errorToken, string.Empty);
        }
        
        var loginTokens = await GenerateAndSaveTokenAsync(identityUser);
        return new AuthResult(true, new List<string>(), loginTokens.TokenString, loginTokens.RefreshTokenString);
    }

    private async Task<GeneratedTokensData> GenerateAndSaveTokenAsync(IdentityUser user)
    {
        var token = new TokenUserDto(user.Id, user.Email);
        var tokenString = _tokenService.GenerateToken(token);
        var refreshTokenString = _tokenService.GenerateRefreshToken(); // generating string for refreshToken
        var refreshToken =
            new RefreshToken(refreshTokenString, DateTime.UtcNow.AddDays(30),
                user.Id); //creating new object of refresh token
        await _repository.AddAsync(refreshToken);
        return new GeneratedTokensData(tokenString, refreshTokenString);
    }
}