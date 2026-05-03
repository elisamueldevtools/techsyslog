using Microsoft.AspNetCore.SignalR;
using TechsysLog.Application.Common.Interfaces;
using TechsysLog.Domain.Entities;

namespace TechsysLog.Infrastructure.Realtime;

public class SignalRRealtimeNotifier : IRealtimeNotifier
{
    private readonly IHubContext<NotificationHub> _hub;

    public SignalRRealtimeNotifier(IHubContext<NotificationHub> hub) { _hub = hub; }

    public Task PublishAsync(string eventName, object payload, CancellationToken ct) =>
        _hub.Clients.All.SendAsync(eventName, payload, ct);

    public Task PublishNotificationAsync(Notification notification, CancellationToken ct) =>
        _hub.Clients.All.SendAsync("Notification", new
        {
            notification.Id,
            notification.Type,
            notification.Message,
            notification.Read,
            notification.CreatedAt
        }, ct);
}
