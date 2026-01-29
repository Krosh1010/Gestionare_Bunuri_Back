using Application.Abstraction;
using Domain.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gestionare_Bunuri_Back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            var token = await _userService.LoginAsync(dto);
            if (token == null)
                return Unauthorized(new { message = "Invalid credentials" });

            return Ok(new { token });
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            try
            {
                var token = await _userService.RegisterAsync(dto);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                if (ex.Message == "User existent")
                    return Conflict(new { message = "User already exists" });

                // Afișează mesajul real al excepției pentru debugging
                return StatusCode(500, new { message = ex.Message, stack = ex.StackTrace });
            }
        }
    }
}
