using TechsysLog.Domain.Entities;

namespace TechsysLog.Application.Common.Interfaces;

public interface IRealtimeNotifier
{
    Task PublishAsync(string eventName, object payload, CancellationToken ct);
    Task PublishNotificationAsync(Notification notification, CancellationToken ct);
}
