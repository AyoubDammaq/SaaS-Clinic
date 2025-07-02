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

        // POST: api/notification
        [HttpPost]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _notificationService.CreateNotificationAsync(request);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
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

        // POST: api/notification/send
        [HttpPost("send")]
        public async Task<IActionResult> SendNotification([FromBody] SendNotificationRequest request)
        {
            await _notificationService.SendNotificationAsync(request);
            return Ok();
        }

        // PUT: api/notification/mark-as-sent
        [HttpPut("mark-as-sent")]
        public async Task<IActionResult> MarkAsSent([FromBody] MarkNotificationAsSentRequest request)
        {
            await _notificationService.MarkAsSentAsync(request);
            return Ok();
        }

        /*
        // GET: api/notification/preferences/{userId}?userType=...
        [HttpGet("preferences/{userId:guid}")]
        public async Task<ActionResult<NotificationPreferenceDto>> GetPreferences(Guid userId, [FromQuery] UserType userType)
        {
            var prefs = await _notificationService.GetUserPreferencesAsync(userId, userType);
            if (prefs == null)
                return NotFound();

            return Ok(prefs);
        }

        // PUT: api/notification/preferences
        [HttpPut("preferences")]
        public async Task<IActionResult> UpdatePreferences([FromBody] UpdateNotificationPreferenceRequest request)
        {
            await _notificationService.UpdateUserPreferencesAsync(request);
            return NoContent();
        }
        */
    }
}
