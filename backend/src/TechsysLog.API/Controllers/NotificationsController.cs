using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechsysLog.Application.Features.Notifications.GetNotifications;
using TechsysLog.Application.Features.Notifications.MarkAsRead;

namespace TechsysLog.API.Controllers;

[ApiController]
[Authorize]
[Route("notifications")]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator) { _mediator = mediator; }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<NotificationListItem>>> List(CancellationToken ct)
        => Ok(await _mediator.Send(new GetNotificationsQuery(), ct));

    [HttpPost("{id:guid}/read")]
    public async Task<ActionResult<MarkNotificationReadResponse>> MarkAsRead(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new MarkNotificationReadCommand(id), ct));
}
