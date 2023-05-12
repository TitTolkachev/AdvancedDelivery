using Microsoft.AspNetCore.Mvc;
using Notifications.Common.Dto;
using Notifications.Common.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace Notifications.API.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationsService _notificationsService;

    public NotificationsController(INotificationsService notificationsService)
    {
        _notificationsService = notificationsService;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Post notification")]
    public async Task<ActionResult> PostNotification([FromBody] NotificationReceived bodyData)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await _notificationsService.AcceptAndSend(bodyData);

        return Ok();
    }
}