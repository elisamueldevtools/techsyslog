using MongoDB.Driver;
using TechsysLog.Application.Common.Interfaces;
using TechsysLog.Domain.Entities;

namespace TechsysLog.Infrastructure.Persistence.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly MongoContext _ctx;

    public NotificationRepository(MongoContext ctx) { _ctx = ctx; }

    public Task AddAsync(Notification notification, CancellationToken ct) =>
        _ctx.Notifications.InsertOneAsync(notification, cancellationToken: ct);

    public async Task<IReadOnlyList<Notification>> GetAllAsync(CancellationToken ct)
    {
        var list = await _ctx.Notifications.Find(FilterDefinition<Notification>.Empty)
            .SortByDescending(n => n.CreatedAt)
            .ToListAsync(ct);
        return list;
    }

    public Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _ctx.Notifications.Find(n => n.Id == id).FirstOrDefaultAsync(ct)!;

    public Task UpdateAsync(Notification notification, CancellationToken ct) =>
        _ctx.Notifications.ReplaceOneAsync(n => n.Id == notification.Id, notification, cancellationToken: ct);
}
