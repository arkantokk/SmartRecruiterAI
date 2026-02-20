using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmartRecruiter.Application.DTOs;
using SmartRecruiter.Application.Interfaces;

namespace SmartRecruiter.Infrastructure.Services;

public class IdentityAuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;


    public IdentityAuthService(UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        _configuration = configuration;
        _userManager = userManager;
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        var user = new IdentityUser { UserName = request.Email, Email = request.Email };
        var result = await _userManager.CreateAsync(user, request.Password);
        if (result.Succeeded)
        {
            return new AuthResult(true, new List<string>());
        }

        var errorMessages = result.Errors.Select(e => e.Description).ToList();

        return new AuthResult(result.Succeeded, errorMessages);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) throw new UnauthorizedAccessException("Invalid email or password.");
        var result = await _userManager.CheckPasswordAsync(user, request.Password);
        if (result)
        {
            // creating jwt token for user
            var claims = new List<Claim>
            {
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
                expires: DateTime.Now.AddHours(3), // The badge expires in 3 hours
                claims: claims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
            // object to string token for webpages
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new AuthResponse(tokenString);
        }
        else
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }
    }
}