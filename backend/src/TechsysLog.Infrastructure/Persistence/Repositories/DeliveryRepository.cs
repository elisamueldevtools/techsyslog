using MongoDB.Driver;
using TechsysLog.Application.Common.Interfaces;
using TechsysLog.Domain.Entities;

namespace TechsysLog.Infrastructure.Persistence.Repositories;

public class DeliveryRepository : IDeliveryRepository
{
    private readonly MongoContext _ctx;

    public DeliveryRepository(MongoContext ctx) { _ctx = ctx; }

    public Task AddAsync(Delivery delivery, CancellationToken ct) =>
        _ctx.Deliveries.InsertOneAsync(delivery, cancellationToken: ct);

    public async Task<IReadOnlyList<Delivery>> GetByOrderIdAsync(Guid orderId, CancellationToken ct)
    {
        var list = await _ctx.Deliveries.Find(d => d.OrderId == orderId)
            .SortBy(d => d.DeliveredAt)
            .ToListAsync(ct);
        return list;
    }
}
