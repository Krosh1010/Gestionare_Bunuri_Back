using Application.Abstraction;
using Microsoft.AspNetCore.Mvc;
using Domain.User;

namespace Gestionare_Bunuri_Back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetUserData()
        {
            var userIdString = HttpContext.Items["UserId"] as string;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            int userId = int.Parse(userIdString);

            var user = await _userService.GetUserInfoAsync(userId);
            if (user == null)
                return NotFound();

            return Ok(user);
        }
        [HttpPatch("update-data")]
        public async Task<IActionResult> PatchUser([FromBody] UserUpdateDataDto dto)
        {
            var userIdString = HttpContext.Items["UserId"] as string;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            int userId = int.Parse(userIdString);

            var updatedUser = await _userService.PatchUserDataAsync(userId, dto);
            if (updatedUser == null)
                return NotFound();

            return Ok(updatedUser);
        }

        [HttpPatch("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] UserChangePasswordDto dto)
        {
            var userIdString = HttpContext.Items["UserId"] as string;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            int userId = int.Parse(userIdString);

            var result = await _userService.ChangePasswordAsync(userId, dto);
            if (!result)
                return BadRequest(new { message = "Parola curentă este incorectă." });

            return Ok(new { message = "Parola a fost schimbată cu succes." });
        }
    }
}
