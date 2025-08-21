using InventoryMgt.Api.Services;
using InventoryMgt.Data.Mappers;
using InventoryMgt.Shared.Contracts;
using InventoryMgt.Shared.CustomExceptions;
using InventoryMgt.Shared.DTOs;
using InventoryMgt.Shared.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace InventoryMgt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;
        private readonly ITokenService _tokenService;
        private readonly ITokenInfoRepository _tokenInfoRepository;

        public AuthController(IAuthRepository authRepo, ITokenService tokenService, ITokenInfoRepository tokenInfoRepository)
        {
            _authRepo = authRepo;
            _tokenService = tokenService;
            _tokenInfoRepository = tokenInfoRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginData)
        {
            var user = await _authRepo.LoginAsync(loginData);

            List<Claim> claims = [
                new (ClaimTypes.Name, user.Username),  // claim to store name
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            // unique identifier for jwt
            ];

            // adding role to claims

            claims.Add(new Claim(ClaimTypes.Role, user.Role));

            // generating access and refresh token
            var accessToken = _tokenService.GenerateAccessToken(claims);
            string refreshToken = _tokenService.GenerateRefreshToken();

            var tokenInfo = await _tokenInfoRepository.GetTokenInfoByUsernameAsync(user.Username);

            if (tokenInfo == null)
            {
                var tokenInfoDto = new CreateTokenInfoDto
                {
                    Username = user.Username,
                    RefreshToken = refreshToken,
                    ExpiredAt = DateTime.UtcNow.AddMinutes(2) // TODO: Change to 7 days
                };
                await _tokenInfoRepository.AddTokenInfoAsync(tokenInfoDto);
            }
            else
            {
                var tokenInfoToUpdate = tokenInfo.ToUpdateTokenInfoDto();
                tokenInfoToUpdate.RefreshToken = refreshToken;
                tokenInfoToUpdate.ExpiredAt = DateTime.UtcNow.AddMinutes(2);// TODO: Change to 7 days
                await _tokenInfoRepository.UpdateTokenInfoAsync(tokenInfoToUpdate);
            }

            var tokenModel = new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            // set token cookies

            _tokenService.SetTokenCookies(tokenModel, HttpContext);

            // also sending it as a response, because cookie don't work with mobile app clients
            return Ok(tokenModel);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(TokenModel tokenModel)
        {
            HttpContext.Request.Cookies.TryGetValue("accessToken", out var accessToken);
            HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken);

            if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
            {
                tokenModel.AccessToken = accessToken;
                tokenModel.RefreshToken = refreshToken;
            }

            var principal = _tokenService.GetPrincipalFromExpiredToken(tokenModel.AccessToken) ?? throw new BadRequestException("Invalid expired token");

            var username = principal.Identity?.Name ?? "";

            var tokenInfo = await _tokenInfoRepository.GetTokenInfoByUsernameAsync(username);
            if (tokenInfo == null
            || tokenInfo.RefreshToken != tokenModel.RefreshToken
            || tokenInfo.ExpiredAt <= DateTime.UtcNow)
            {
                return BadRequest("Invalid refresh token. Please login again.");
            }

            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            tokenInfo.RefreshToken = newRefreshToken; // rotating the refresh token
            await _tokenInfoRepository.UpdateTokenInfoAsync(tokenInfo.ToUpdateTokenInfoDto());

            var newTokenData = new TokenModel
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };

            // set token cookies

            _tokenService.SetTokenCookies(newTokenData, HttpContext);

            // also sending it as a response, because cookie don't work with mobile app clients
            return Ok(newTokenData);

        }


        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyInfo()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                throw new UnauthorizedException("You are not authorized");
            }
            var user = await _authRepo.GetUserByUsernameAsync(username);
            return Ok(user);
        }
    }
}
