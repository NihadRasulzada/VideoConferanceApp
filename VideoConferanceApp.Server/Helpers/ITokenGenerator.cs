using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace VideoConferanceApp.Server.Helpers;

public interface ITokenGenerator
{
    string GenerateToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
}

public class TokenGenerator(IConfiguration config) : ITokenGenerator
{
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using (var rng = RandomNumberGenerator.Create())
            rng.GetBytes(randomNumber);

        // Convert to a Base64 string so it's easily stored and transmitted
        return Convert.ToBase64String(randomNumber);
    }

    public string GenerateToken(IEnumerable<Claim> claims)
    {
        var securityKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));

        var credentials =
            new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}