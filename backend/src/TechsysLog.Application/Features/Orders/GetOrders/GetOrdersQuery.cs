using MediatR;
using TechsysLog.Domain.Enums;

namespace TechsysLog.Application.Features.Orders.GetOrders;

public record GetOrdersQuery(OrderStatus? Status = null) : IRequest<IReadOnlyList<OrderListItem>>;

public record OrderListItem(
    Guid Id,
    string OrderNumber,
    OrderStatus Status,
    decimal Value,
    DateTime CreatedAt);
