using MediatR;
using TechsysLog.Application.Common.Interfaces;

namespace TechsysLog.Application.Features.Notifications.GetNotifications;

public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, IReadOnlyList<NotificationListItem>>
{
    private readonly INotificationRepository _notifications;

    public GetNotificationsQueryHandler(INotificationRepository notifications)
    {
        _notifications = notifications;
    }

    public async Task<IReadOnlyList<NotificationListItem>> Handle(GetNotificationsQuery request, CancellationToken ct)
    {
        var items = await _notifications.GetAllAsync(ct);
        return items
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationListItem(n.Id, n.Type, n.Message, n.Read, n.CreatedAt))
            .ToList();
    }
}
