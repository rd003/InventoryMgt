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

        public AuthController(IAuthRepository authRepo)
        {
            _authRepo = authRepo;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginData)
        {
            var user = await _authRepo.LoginAsync(loginData);
            return Ok(user);
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
