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
            try
            {
                var token = await _userService.LoginAsync(dto);
                if (token == null)
                    return Unauthorized(new { message = "Credențiale invalide." });

                return Ok(new { token });
            }
            catch (Exception ex) when (ex.Message == "Email neverificat")
            {
                return StatusCode(403, new { message = "Email-ul nu a fost verificat. Verifică-ți inbox-ul." });
            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            try
            {
                await _userService.RegisterAsync(dto);
                return Ok(new { message = "Cont creat. Verifică email-ul pentru codul de confirmare." });
            }
            catch (Exception ex)
            {
                if (ex.Message == "User existent")
                    return Conflict(new { message = "Există deja un cont cu acest email." });

                return StatusCode(500, new { message = ex.Message, stack = ex.StackTrace });
            }
        }

        [AllowAnonymous]
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto dto)
        {
            var token = await _userService.VerifyEmailAsync(dto);
            if (token == null)
                return BadRequest(new { message = "Codul de verificare este invalid sau a expirat." });

            return Ok(new { token });
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
