using Domain.DbTables;
using Microsoft.AspNetCore.Mvc;

namespace Gestionare_Bunuri_Back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var userIdString = HttpContext.Items["UserId"] as string;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            int userId = int.Parse(userIdString);

            await _notificationService.GenerateExpiringNotificationsAsync(userId);

            var notifications = await _notificationService.GetNotificationsByUserIdAsync(userId);

            // Proiectează doar datele necesare pentru dashboard
            var result = notifications.Select(n => new
            {
                n.Id,
                n.Type,
                n.Message,
                n.IsRead,
            });

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var userIdString = HttpContext.Items["UserId"] as string;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            int userId = int.Parse(userIdString);

            var deleted = await _notificationService.DeleteNotificationAsync(id, userId);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
