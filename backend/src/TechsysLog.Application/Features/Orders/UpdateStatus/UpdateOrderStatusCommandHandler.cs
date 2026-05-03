using MediatR;
using TechsysLog.Application.Common.Interfaces;
using TechsysLog.Domain.Entities;
using TechsysLog.Domain.Enums;
using TechsysLog.Domain.Exceptions;

namespace TechsysLog.Application.Features.Orders.UpdateStatus;

public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, UpdateOrderStatusResponse>
{
    private readonly IOrderRepository _orders;
    private readonly INotificationRepository _notifications;
    private readonly IRealtimeNotifier _realtime;

    public UpdateOrderStatusCommandHandler(
        IOrderRepository orders,
        INotificationRepository notifications,
        IRealtimeNotifier realtime)
    {
        _orders = orders;
        _notifications = notifications;
        _realtime = realtime;
    }

    public async Task<UpdateOrderStatusResponse> Handle(UpdateOrderStatusCommand request, CancellationToken ct)
    {
        var order = await _orders.GetByIdAsync(request.OrderId, ct)
                    ?? throw new NotFoundException(nameof(Order), request.OrderId);

        order.ChangeStatus(request.Status);
        await _orders.UpdateAsync(order, ct);

        var notification = new Notification
        {
            Type = NotificationType.OrderStatusChanged,
            Message = $"Pedido {order.OrderNumber} mudou para {order.Status}.",
            Read = false
        };
        await _notifications.AddAsync(notification, ct);
        await _realtime.PublishNotificationAsync(notification, ct);
        await _realtime.PublishAsync("OrderStatusChanged", new { order.Id, order.OrderNumber, order.Status }, ct);

        return new UpdateOrderStatusResponse(true);
    }
}
