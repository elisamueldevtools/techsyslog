using MediatR;
using TechsysLog.Domain.Enums;

namespace TechsysLog.Application.Features.Orders.UpdateStatus;

public record UpdateOrderStatusCommand(Guid OrderId, OrderStatus Status) : IRequest<UpdateOrderStatusResponse>;

public record UpdateOrderStatusResponse(bool Success);
