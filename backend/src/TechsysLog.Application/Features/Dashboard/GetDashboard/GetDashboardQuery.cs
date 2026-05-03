using MediatR;
using TechsysLog.Domain.Enums;

namespace TechsysLog.Application.Features.Dashboard.GetDashboard;

public record GetDashboardQuery(int Month, int Year) : IRequest<DashboardResponse>;

public record DashboardOrderItem(
    Guid Id,
    string OrderNumber,
    decimal Value,
    DateTime CreatedAt);

public record DashboardResponse(
    int Month,
    int Year,
    Dictionary<OrderStatus, int> Counters,
    Dictionary<OrderStatus, IReadOnlyList<DashboardOrderItem>> Grids);
