using MediatR;
using TechsysLog.Application.Common.Interfaces;
using TechsysLog.Domain.Entities;
using TechsysLog.Domain.Exceptions;

namespace TechsysLog.Application.Features.Notifications.MarkAsRead;

public class MarkNotificationReadCommandHandler
    : IRequestHandler<MarkNotificationReadCommand, MarkNotificationReadResponse>
{
    private readonly INotificationRepository _notifications;

    public MarkNotificationReadCommandHandler(INotificationRepository notifications)
    {
        _notifications = notifications;
    }

    public async Task<MarkNotificationReadResponse> Handle(MarkNotificationReadCommand request, CancellationToken ct)
    {
        var notification = await _notifications.GetByIdAsync(request.Id, ct)
                            ?? throw new NotFoundException(nameof(Notification), request.Id);

        if (!notification.Read)
        {
            notification.Read = true;
            await _notifications.UpdateAsync(notification, ct);
        }

        return new MarkNotificationReadResponse(true);
    }
}
