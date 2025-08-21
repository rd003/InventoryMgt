using System.Security.Claims;
using InventoryMgt.Shared.DTOs;

namespace InventoryMgt.Api.Services;

public interface ITokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken);
    void SetTokenCookies(TokenModel tokenModel, HttpContext context);
}
