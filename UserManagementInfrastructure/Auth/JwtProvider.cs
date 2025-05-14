using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UserManagementCore.Contracts;
using UserManagementCore.Models;

namespace Infrastructure.Auth;

public class JwtProvider(IOptions<JwtOptions> jwtOptions) : IJwtProvider
{
    private readonly JwtOptions _options = jwtOptions.Value;

    public string GenerateToken(UserModel user)
    {
        Claim[] claims = [new("Id", user.Id.ToString())];
        
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: signingCredentials,
            expires: DateTime.UtcNow.AddHours(_options.ExpiresHours));
        
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        
        return tokenString;
    }
}