using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmartRecruiter.Application.Interfaces;
using SmartRecruiter.Domain.DTOs;

namespace SmartRecruiter.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    
    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(TokenUserDto user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name,
                user.Email), // list of claims(or what is included in this token email, role, other)
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var keyBytes = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
        var authSigningKey = new SymmetricSecurityKey(keyBytes);
        // creating token 
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            expires: DateTime.UtcNow.AddMinutes(1), // The badge expires in 3 hours
            claims: claims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );
        // token object to string token for webpages
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenString;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64]; // creating number that has 64 bytes
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber); // fills with random bytes
        return Convert.ToBase64String(randomNumber); // converting to string
    }
}