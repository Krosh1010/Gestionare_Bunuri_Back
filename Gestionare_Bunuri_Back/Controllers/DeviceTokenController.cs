using Application.Abstraction;
using Domain.DeviceToken;
using Microsoft.AspNetCore.Mvc;

namespace Gestionare_Bunuri_Back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceTokenController : ControllerBase
    {
        private readonly IDeviceTokenService _deviceTokenService;

        public DeviceTokenController(IDeviceTokenService deviceTokenService)
        {
            _deviceTokenService = deviceTokenService;
        }

        /// <summary>
        /// Înregistrează un device token FCM pentru push notifications.
        /// Apelat de aplicația mobilă la login sau la pornirea aplicației.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterToken([FromBody] DeviceTokenRegisterDto dto)
        {
            var userIdString = HttpContext.Items["UserId"] as string;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            int userId = int.Parse(userIdString);

            if (string.IsNullOrWhiteSpace(dto.Token))
                return BadRequest("Token-ul este obligatoriu.");

            await _deviceTokenService.RegisterTokenAsync(userId, dto.Token, dto.Platform ?? "android");

            return Ok(new { message = "Device token înregistrat cu succes." });
        }

        /// <summary>
        /// Șterge un device token FCM (de exemplu la logout).
        /// </summary>
        [HttpPost("unregister")]
        public async Task<IActionResult> UnregisterToken([FromBody] DeviceTokenRegisterDto dto)
        {
            var userIdString = HttpContext.Items["UserId"] as string;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            int userId = int.Parse(userIdString);

            if (string.IsNullOrWhiteSpace(dto.Token))
                return BadRequest("Token-ul este obligatoriu.");

            await _deviceTokenService.RemoveTokenAsync(userId, dto.Token);

            return Ok(new { message = "Device token șters cu succes." });
        }
    }
}
