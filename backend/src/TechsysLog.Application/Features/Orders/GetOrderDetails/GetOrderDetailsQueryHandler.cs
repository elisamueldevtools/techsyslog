using MediatR;
using TechsysLog.Application.Common.Interfaces;
using TechsysLog.Domain.Entities;
using TechsysLog.Domain.Exceptions;

namespace TechsysLog.Application.Features.Orders.GetOrderDetails;

public class GetOrderDetailsQueryHandler : IRequestHandler<GetOrderDetailsQuery, OrderDetailsResponse>
{
    private readonly IOrderRepository _orders;
    private readonly IDeliveryRepository _deliveries;

    public GetOrderDetailsQueryHandler(IOrderRepository orders, IDeliveryRepository deliveries)
    {
        _orders = orders;
        _deliveries = deliveries;
    }

    public async Task<OrderDetailsResponse> Handle(GetOrderDetailsQuery request, CancellationToken ct)
    {
        var order = await _orders.GetByIdAsync(request.Id, ct)
                    ?? throw new NotFoundException(nameof(Order), request.Id);

        var deliveries = await _deliveries.GetByOrderIdAsync(order.Id, ct);

        var addressDto = new AddressDto(
            order.Address.Cep,
            order.Address.Street,
            order.Address.Number,
            order.Address.Complement,
            order.Address.Neighborhood,
            order.Address.City,
            order.Address.State);

        var orderDto = new OrderDetailsDto(
            order.Id,
            order.OrderNumber,
            order.Description,
            order.Value,
            order.Status,
            addressDto,
            order.Observation,
            order.CreatedAt);

        var deliveryDtos = deliveries
            .Select(d => new DeliveryDetailsDto(d.Id, d.DeliveredAt, d.Notes))
            .ToList();

        return new OrderDetailsResponse(orderDto, deliveryDtos);
    }
}
