using Microsoft.AspNetCore.Mvc;
using Notif.Application.DTOs;
using Notif.Application.Interfaces;

namespace Notif.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationApplicationService _notificationService;

        public NotificationController(INotificationApplicationService notificationService)
        {
            _notificationService = notificationService;
        }

        // GET: api/notification/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<NotificationDto>> GetById(Guid id)
        {
            var notification = await _notificationService.GetNotificationByIdAsync(id);
            if (notification == null)
                return NotFound();

            return Ok(notification);
        }

        // GET: api/notification?recipientId=...&status=...&page=1&pageSize=20
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationSummaryDto>>> GetNotifications([FromQuery] NotificationFilterRequest filter)
        {
            var result = await _notificationService.GetNotificationsAsync(filter);
            return Ok(result);
        }

        // PUT: api/notification/mark-as-sent
        [HttpPut("mark-as-sent")]
        public async Task<IActionResult> MarkAsSent([FromBody] MarkNotificationAsSentRequest request)
        {
            await _notificationService.MarkAsSentAsync(request);
            return Ok();
        }

        // GET: api/notification/recipient/{recipientId}
        [HttpGet("recipient/{recipientId:guid}")]
        public async Task<ActionResult<List<NotificationDto>>> GetNotificationsByRecipientId(Guid recipientId)
        {
            var notifications = await _notificationService.GetNotificationsByRecipientId(recipientId);
            return Ok(notifications);
        }

        // DELETE: api/notification/{notificationId}
        [HttpDelete("{notificationId:guid}")]
        public async Task<IActionResult> DeleteNotification(Guid notificationId)
        {
            await _notificationService.DeleteNotification(notificationId);
            return NoContent();
        }

        // DELETE: api/notification/recipient/{recipientId}
        [HttpDelete("recipient/{recipientId:guid}")]
        public async Task<IActionResult> DeleteAllNotifications(Guid recipientId)
        {
            await _notificationService.DeleteAllNotifications(recipientId);
            return NoContent();
        }

        // PUT: api/notification/mark-as-read/{notificationId}
        [HttpPut("mark-as-read/{notificationId:guid}")]
        public async Task<IActionResult> MarkNotificationAsRead(Guid notificationId)
        {
            var notification = await _notificationService.GetNotificationByIdAsync(notificationId);
            if (notification == null)
                return NotFound();

            await _notificationService.MarkNotificationAsRead(notification);

            return Ok();
        }

        // PUT: api/notification/mark-all-as-read/{recipientId}
        [HttpPut("mark-all-as-read/{recipientId:guid}")]
        public async Task<IActionResult> MarkAllNotificationsAsRead(Guid recipientId)
        {
            await _notificationService.MarkAllNotificationsAsRead(recipientId);
            return Ok();
        }

        [HttpGet("test")]
        public IActionResult Test() => Ok("API is running");
    }
}
