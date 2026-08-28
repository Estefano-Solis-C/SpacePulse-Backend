using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using RentalPeAPI.User.Domain;
using RentalPeAPI.User.Domain.Services;

namespace RentalPeAPI.User.Infrastructure.Security;

public class TokenGenerationService : ITokenGenerationService
{
    private readonly IConfiguration _configuration;

    public TokenGenerationService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(Domain.User user)
    {
        var validRoles = new[] { "Homeowner", "Remodeler" };
        if (!validRoles.Contains(user.Role, StringComparer.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException(
                $"Usuario con rol '{user.Role}' no autorizado. Solo se permiten roles: {string.Join(", ", validRoles)}");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role), 
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}