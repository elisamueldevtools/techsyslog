using TechsysLog.Domain.Common;
using TechsysLog.Domain.Enums;
using TechsysLog.Domain.Exceptions;

namespace TechsysLog.Domain.Entities;

public class Order : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Created;
    public Address Address { get; set; } = new();
    public string? Observation { get; set; }

    public void ChangeStatus(OrderStatus next)
    {
        if (next == Status)
            return;

        if (!IsValidTransition(Status, next))
            throw new DomainException($"Invalid status transition: {Status} -> {next}.");

        Status = next;
    }

    public bool MarkAsDelivered()
    {
        if (Status == OrderStatus.Cancelled)
            throw new DomainException("Não é possível registrar entrega de um pedido cancelado.");

        if (Status == OrderStatus.Delivered)
            return false;

        Status = OrderStatus.Delivered;
        return true;
    }

    private static bool IsValidTransition(OrderStatus current, OrderStatus next) => current switch
    {
        OrderStatus.Created    => next is OrderStatus.Processing or OrderStatus.Cancelled,
        OrderStatus.Processing => next is OrderStatus.Shipped    or OrderStatus.Cancelled,
        OrderStatus.Shipped    => next is OrderStatus.Delivered  or OrderStatus.Cancelled,
        OrderStatus.Delivered  => false,
        OrderStatus.Cancelled  => false,
        _ => false
    };
}
