using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using InventoryMgt.Api.Services;
using InventoryMgt.Data.CustomExceptions;
using InventoryMgt.Data.models.DTOs;
using InventoryMgt.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMgt.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;
        private readonly ITokenService _tokenService;

        public AuthController(IAuthRepository authRepo, ITokenService tokenService)
        {
            _authRepo = authRepo;
            _tokenService = tokenService;
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

            // generating access token
            var token = _tokenService.GenerateAccessToken(claims);
            return Ok(token);
        }

        [Authorize]
        [HttpPost("me")]
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
