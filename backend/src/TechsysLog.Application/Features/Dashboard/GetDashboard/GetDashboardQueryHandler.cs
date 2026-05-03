using MediatR;
using TechsysLog.Application.Common.Interfaces;
using TechsysLog.Domain.Enums;

namespace TechsysLog.Application.Features.Dashboard.GetDashboard;

public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, DashboardResponse>
{
    private const int GridLimit = 10;
    private static readonly OrderStatus[] AllStatuses = Enum.GetValues<OrderStatus>();

    private readonly IOrderRepository _orders;

    public GetDashboardQueryHandler(IOrderRepository orders)
    {
        _orders = orders;
    }

    public async Task<DashboardResponse> Handle(GetDashboardQuery request, CancellationToken ct)
    {
        var start = new DateTime(request.Year, request.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var end = start.AddMonths(1);

        var allOrders = await _orders.GetAllAsync(ct);
        var inRange = allOrders
            .Where(o => o.CreatedAt >= start && o.CreatedAt < end)
            .ToList();

        var counters = AllStatuses.ToDictionary(s => s, _ => 0);
        var grids = AllStatuses.ToDictionary(
            s => s,
            _ => (IReadOnlyList<DashboardOrderItem>)Array.Empty<DashboardOrderItem>());

        foreach (var status in AllStatuses)
        {
            var ordersOfStatus = inRange.Where(o => o.Status == status).ToList();
            counters[status] = ordersOfStatus.Count;

            grids[status] = ordersOfStatus
                .Select(o => new DashboardOrderItem(o.Id, o.OrderNumber, o.Value, o.CreatedAt))
                .OrderByDescending(item => item.CreatedAt)
                .Take(GridLimit)
                .ToList();
        }

        return new DashboardResponse(request.Month, request.Year, counters, grids);
    }
}
