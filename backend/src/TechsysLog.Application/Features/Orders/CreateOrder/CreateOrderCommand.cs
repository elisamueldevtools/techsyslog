using MediatR;
using TechsysLog.Domain.Enums;

namespace TechsysLog.Application.Features.Orders.CreateOrder;

public record CreateOrderCommand(
    string OrderNumber,
    string Description,
    decimal Value,
    string Cep,
    string Number,
    string? Complement = null,
    string? Observation = null) : IRequest<CreateOrderResponse>;

public record CreateOrderResponse(Guid Id, OrderStatus Status);
