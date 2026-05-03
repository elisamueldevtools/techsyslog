using MediatR;
using TechsysLog.Application.Common.Interfaces;
using TechsysLog.Domain.Entities;
using TechsysLog.Domain.Enums;
using TechsysLog.Domain.Exceptions;

namespace TechsysLog.Application.Features.Orders.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
{
    private readonly IOrderRepository _orders;
    private readonly INotificationRepository _notifications;
    private readonly ICepService _cep;
    private readonly IRealtimeNotifier _realtime;

    public CreateOrderCommandHandler(
        IOrderRepository orders,
        INotificationRepository notifications,
        ICepService cep,
        IRealtimeNotifier realtime)
    {
        _orders = orders;
        _notifications = notifications;
        _cep = cep;
        _realtime = realtime;
    }

    public async Task<CreateOrderResponse> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        var existing = await _orders.GetByOrderNumberAsync(request.OrderNumber, ct);
        if (existing is not null)
            throw new ConflictException("Pedido com este número já existe.");

        var address = await _cep.LookupAsync(request.Cep, ct)
                       ?? throw new DomainException("CEP not found.");
        address.Number = request.Number;
        address.Complement = request.Complement;

        var order = new Order
        {
            OrderNumber = request.OrderNumber,
            Description = request.Description,
            Value = request.Value,
            Status = OrderStatus.Created,
            Address = address,
            Observation = request.Observation
        };
        await _orders.AddAsync(order, ct);

        var notification = new Notification
        {
            Type = NotificationType.OrderCreated,
            Message = $"Pedido {order.OrderNumber} criado.",
            Read = false
        };
        await _notifications.AddAsync(notification, ct);
        await _realtime.PublishNotificationAsync(notification, ct);
        await _realtime.PublishAsync("OrderCreated", new { order.Id, order.OrderNumber, order.Status }, ct);

        return new CreateOrderResponse(order.Id, order.Status);
    }
}
