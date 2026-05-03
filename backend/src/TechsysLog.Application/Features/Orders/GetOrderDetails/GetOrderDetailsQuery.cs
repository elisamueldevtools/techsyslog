using MediatR;
using TechsysLog.Domain.Enums;

namespace TechsysLog.Application.Features.Orders.GetOrderDetails;

public record GetOrderDetailsQuery(Guid Id) : IRequest<OrderDetailsResponse>;

public record AddressDto(
    string Cep,
    string Street,
    string Number,
    string? Complement,
    string Neighborhood,
    string City,
    string State);

public record OrderDetailsDto(
    Guid Id,
    string OrderNumber,
    string Description,
    decimal Value,
    OrderStatus Status,
    AddressDto Address,
    string? Observation,
    DateTime CreatedAt);

public record DeliveryDetailsDto(
    Guid Id,
    DateTime DeliveredAt,
    string Notes);

public record OrderDetailsResponse(
    OrderDetailsDto Order,
    IReadOnlyList<DeliveryDetailsDto> Deliveries);
