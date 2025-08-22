using InventoryMgt.Shared.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace InventoryMgt.Api.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        // Create a symmetric security key using the secret key from the configuration.
        var authSigningKey = new SymmetricSecurityKey
                        (Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _configuration["JWT:ValidIssuer"],
            Audience = _configuration["JWT:ValidAudience"],
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddMinutes(1),
            SigningCredentials = new SigningCredentials
                          (authSigningKey, SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString("N");
    }

    public IEnumerable<Claim> GenerateClaims(string username, string role)
    {
        List<Claim> claims = [
            new (ClaimTypes.Name, username),  // claim to store name
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        // unique identifier for jwt
        ];

        // adding role to claims

        claims.Add(new Claim(ClaimTypes.Role, role));

        return claims;
    }

    public void SetTokenCookies(TokenModel tokenModel, HttpContext context)
    {
        context.Response.Cookies.Append("accessToken", tokenModel.AccessToken, new CookieOptions
        {
            Expires = DateTime.UtcNow.AddMinutes(15),  // TODO : set to 15 min
            HttpOnly = true,
            IsEssential = true,
            Secure = true,
            Path = "/",
            SameSite = SameSiteMode.None // TODO: set it to strict or lax for production
        });

        context.Response.Cookies.Append("refreshToken", tokenModel.RefreshToken, new CookieOptions
        {
            Expires = DateTime.UtcNow.AddDays(30),
            HttpOnly = true,
            IsEssential = true,
            Secure = true,
            Path = "/",
            SameSite = SameSiteMode.None // TODO: set it to strict or lax for production
        });
    }
}
