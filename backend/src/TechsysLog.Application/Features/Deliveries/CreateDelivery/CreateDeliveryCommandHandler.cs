using MediatR;
using TechsysLog.Application.Common.Interfaces;
using TechsysLog.Domain.Entities;
using TechsysLog.Domain.Enums;
using TechsysLog.Domain.Exceptions;

namespace TechsysLog.Application.Features.Deliveries.CreateDelivery;

public class CreateDeliveryCommandHandler : IRequestHandler<CreateDeliveryCommand, CreateDeliveryResponse>
{
    private readonly IOrderRepository _orders;
    private readonly IDeliveryRepository _deliveries;
    private readonly INotificationRepository _notifications;
    private readonly IRealtimeNotifier _realtime;

    public CreateDeliveryCommandHandler(
        IOrderRepository orders,
        IDeliveryRepository deliveries,
        INotificationRepository notifications,
        IRealtimeNotifier realtime)
    {
        _orders = orders;
        _deliveries = deliveries;
        _notifications = notifications;
        _realtime = realtime;
    }

    public async Task<CreateDeliveryResponse> Handle(CreateDeliveryCommand request, CancellationToken ct)
    {
        var order = await _orders.GetByIdAsync(request.OrderId, ct)
                    ?? throw new NotFoundException(nameof(Order), request.OrderId);

        var statusChanged = order.MarkAsDelivered();

        var delivery = new Delivery
        {
            OrderId = order.Id,
            DeliveredAt = request.DeliveredAt,
            Notes = request.Notes ?? string.Empty
        };
        await _deliveries.AddAsync(delivery, ct);

        if (statusChanged)
        {
            await _orders.UpdateAsync(order, ct);
        }

        var deliveryNotification = new Notification
        {
            Type = NotificationType.DeliveryRegistered,
            Message = $"Entrega registrada para o pedido {order.OrderNumber}.",
            Read = false
        };
        await _notifications.AddAsync(deliveryNotification, ct);
        await _realtime.PublishNotificationAsync(deliveryNotification, ct);
        await _realtime.PublishAsync("DeliveryRegistered",
            new { delivery.Id, delivery.OrderId, delivery.DeliveredAt }, ct);

        if (statusChanged)
        {
            var statusNotification = new Notification
            {
                Type = NotificationType.OrderStatusChanged,
                Message = $"Pedido {order.OrderNumber} mudou para Delivered.",
                Read = false
            };
            await _notifications.AddAsync(statusNotification, ct);
            await _realtime.PublishNotificationAsync(statusNotification, ct);
            await _realtime.PublishAsync("OrderStatusChanged",
                new { order.Id, order.OrderNumber, order.Status }, ct);
        }

        return new CreateDeliveryResponse(true);
    }
}
