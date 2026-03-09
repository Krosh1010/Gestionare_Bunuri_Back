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

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var result = await _userService.ForgotPasswordAsync(dto.Email);

            // Întoarcem mereu OK pentru a nu dezvălui dacă email-ul există în sistem
            return Ok(new { message = "Dacă email-ul există în sistem, vei primi un cod de resetare." });
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var result = await _userService.ResetPasswordAsync(dto);
            if (!result)
                return BadRequest(new { message = "Codul de resetare este invalid sau a expirat." });

            return Ok(new { message = "Parola a fost resetată cu succes." });
        }
    }
}
