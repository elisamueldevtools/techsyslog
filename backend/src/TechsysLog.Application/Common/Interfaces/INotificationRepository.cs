using TechsysLog.Domain.Entities;

namespace TechsysLog.Application.Common.Interfaces;

public interface INotificationRepository
{
    Task AddAsync(Notification notification, CancellationToken ct);
    Task<IReadOnlyList<Notification>> GetAllAsync(CancellationToken ct);
    Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct);
    Task UpdateAsync(Notification notification, CancellationToken ct);
}
