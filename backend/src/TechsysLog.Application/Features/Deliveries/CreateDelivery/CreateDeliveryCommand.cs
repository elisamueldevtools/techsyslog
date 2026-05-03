using MediatR;

namespace TechsysLog.Application.Features.Deliveries.CreateDelivery;

public record CreateDeliveryCommand(Guid OrderId, DateTime DeliveredAt, string? Notes) : IRequest<CreateDeliveryResponse>;

public record CreateDeliveryResponse(bool Success);
