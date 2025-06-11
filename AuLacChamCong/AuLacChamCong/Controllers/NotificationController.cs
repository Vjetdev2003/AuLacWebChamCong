using AuLacChamCong.Services.Modules.Notification.Model;
using AuLacChamCong.Services.Modules.Notification.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuLacChamCong.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationServices _notificationServices;
        public NotificationController(INotificationServices notificationServices) {
            _notificationServices = notificationServices;
        }
        [HttpGet("GetAllNotifications")]
        public async Task<IActionResult> GetAllNotifications() {
            try {
                var result = await _notificationServices.GetAllNotifications();
                return Ok(result);
            } catch (Exception ex) {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "Lỗi máy chủ nội bộ.", error = ex.Message });
            }
        }

        [HttpPost("CreateNotification")]
        public async Task<IActionResult> CreateNotification(NotificationDb notification) {
            try {
                if (notification == null) {
                    return BadRequest("Notification cannot be empty.");
                }

                var notificationCreated = await _notificationServices.CreateNotificationAsync(notification);

                if (notificationCreated) {
                    return Ok(new { success = true, message = "Notification created successfully." });
                } else {
                    return Ok(new { success = false, message = "Failed to create notification." });
                }
            } catch (ArgumentException ex) {
                return BadRequest(new { success = false, message = ex.Message });
            } catch (Exception ex) {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "Lỗi máy chủ nội bộ.", error = ex.Message });
            }
        }
        [HttpGet("GetNotificationByPsnPrkID/{psnPrkID}")]
        public async Task<IActionResult> GetNotificationsByPsnPrkIDAsync(int psnPrkID) {
            try {
                var result = await _notificationServices.GetNotificationsByPsnPrkIDAsync(psnPrkID);
                return Ok(result);
            } catch (Exception ex) {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "Lỗi máy chủ nội bộ.", error = ex.Message });
            }
        }
        [HttpGet("MarkAsReadAsync/{notificationID}")]
        public async Task<IActionResult> MarkAsReadAsync(int notificationID) {
            try {
                var result = await _notificationServices.MarkAsReadAsync(notificationID);
                return Ok(result);
            } catch (Exception ex) {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "Lỗi máy chủ nội bộ.", error = ex.Message });
            }
        }
    }
}
