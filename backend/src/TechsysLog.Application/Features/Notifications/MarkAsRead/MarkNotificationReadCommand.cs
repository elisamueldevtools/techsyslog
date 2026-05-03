using MediatR;

namespace TechsysLog.Application.Features.Notifications.MarkAsRead;

public record MarkNotificationReadCommand(Guid Id) : IRequest<MarkNotificationReadResponse>;

public record MarkNotificationReadResponse(bool Success);
