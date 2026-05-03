using MongoDB.Driver;
using TechsysLog.Application.Common.Interfaces;
using TechsysLog.Domain.Entities;

namespace TechsysLog.Infrastructure.Persistence.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly MongoContext _ctx;

    public OrderRepository(MongoContext ctx) { _ctx = ctx; }

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _ctx.Orders.Find(o => o.Id == id).FirstOrDefaultAsync(ct)!;

    public Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken ct) =>
        _ctx.Orders.Find(o => o.OrderNumber == orderNumber).FirstOrDefaultAsync(ct)!;

    public async Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct)
    {
        var list = await _ctx.Orders.Find(FilterDefinition<Order>.Empty)
            .SortByDescending(o => o.CreatedAt)
            .ToListAsync(ct);
        return list;
    }

    public Task AddAsync(Order order, CancellationToken ct) =>
        _ctx.Orders.InsertOneAsync(order, cancellationToken: ct);

    public Task UpdateAsync(Order order, CancellationToken ct) =>
        _ctx.Orders.ReplaceOneAsync(o => o.Id == order.Id, order, cancellationToken: ct);
}
