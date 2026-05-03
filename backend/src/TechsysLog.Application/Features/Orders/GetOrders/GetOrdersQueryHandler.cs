using MediatR;
using TechsysLog.Application.Common.Interfaces;

namespace TechsysLog.Application.Features.Orders.GetOrders;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, IReadOnlyList<OrderListItem>>
{
    private readonly IOrderRepository _orders;

    public GetOrdersQueryHandler(IOrderRepository orders)
    {
        _orders = orders;
    }

    public async Task<IReadOnlyList<OrderListItem>> Handle(GetOrdersQuery request, CancellationToken ct)
    {
        var orders = await _orders.GetAllAsync(ct);
        var filtered = request.Status is null
            ? orders
            : orders.Where(o => o.Status == request.Status.Value);
        return filtered
            .Select(o => new OrderListItem(o.Id, o.OrderNumber, o.Status, o.Value, o.CreatedAt))
            .ToList();
    }
}
