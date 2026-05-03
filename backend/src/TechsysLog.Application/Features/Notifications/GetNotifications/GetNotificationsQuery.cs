using MediatR;
using TechsysLog.Domain.Enums;

namespace TechsysLog.Application.Features.Notifications.GetNotifications;

public record GetNotificationsQuery() : IRequest<IReadOnlyList<NotificationListItem>>;

public record NotificationListItem(Guid Id, NotificationType Type, string Message, bool Read, DateTime CreatedAt);
