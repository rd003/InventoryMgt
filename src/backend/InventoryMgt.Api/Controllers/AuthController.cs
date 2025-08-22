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

            var claims = _tokenService.GenerateClaims(user.Username, user.Role);
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
                    ExpiredAt = DateTime.UtcNow.AddDays(30)
                };
                await _tokenInfoRepository.AddTokenInfoAsync(tokenInfoDto);
            }
            else
            {
                var tokenInfoToUpdate = tokenInfo.ToUpdateTokenInfoDto();
                tokenInfoToUpdate.RefreshToken = refreshToken;
                tokenInfoToUpdate.ExpiredAt = DateTime.UtcNow.AddDays(30);
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
        public async Task<IActionResult> Refresh(TokenRequest tokenModel)
        {
            Console.WriteLine("=== ALL COOKIES ===");
            foreach (var cookie in HttpContext.Request.Cookies)
            {
                Console.WriteLine($"Cookie: {cookie.Key} = {cookie.Value}");
            }
            Console.WriteLine("===================");

            // I can not do auto validation, since I don't need to pass any payload in req body, if I am using it http only cookies
            // But, if this api is used by mobile app client, then we must both value in req body
            tokenModel ??= new TokenRequest();

            HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken);

            if (!string.IsNullOrEmpty(refreshToken))
            {
                tokenModel.RefreshToken = refreshToken;
            }
            // If no cookies, tokenModel should have values from request body (mobile client)
            else if (string.IsNullOrEmpty(tokenModel.RefreshToken))
            {
                throw new BadRequestException("No valid tokens found in cookies or request body");
            }

            var tokenInfo = await _tokenInfoRepository.GetTokenInfoByRefreshTokenAsync(tokenModel.RefreshToken);

            if (tokenInfo == null || tokenInfo.ExpiredAt <= DateTime.UtcNow)
            {
                return BadRequest("Invalid refresh token. Please login again.");
            }

            var user = await _authRepo.GetUserByUsernameAsync(tokenInfo.Username);
            if (user == null)
            {
                return BadRequest("Invalid refresh token. Please login again.");
            }

            var claims = _tokenService.GenerateClaims(user.Username, user.Role);

            var newAccessToken = _tokenService.GenerateAccessToken(claims);
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

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            string? username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                throw new UnauthorizedException("You are not authorized");
            }

            // remove token info from database
            await _tokenInfoRepository.DeleteTokenInfoByUsername(username);

            // remove token cookies
            Response.Cookies.Delete("accessToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Path = "/",
                SameSite = SameSiteMode.None // TODO: change to strict/lax in production
            });

            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Path = "/",
                SameSite = SameSiteMode.None // TODO: change to strict/lax in production
            });

            return NoContent();
        }
    }

}
