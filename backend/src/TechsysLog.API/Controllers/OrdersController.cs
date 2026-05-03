using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechsysLog.Application.Features.Orders.CreateOrder;
using TechsysLog.Application.Features.Orders.GetOrderDetails;
using TechsysLog.Application.Features.Orders.GetOrders;
using TechsysLog.Application.Features.Orders.UpdateStatus;
using TechsysLog.Domain.Enums;

namespace TechsysLog.API.Controllers;

[ApiController]
[Authorize]
[Route("orders")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator) { _mediator = mediator; }

    [HttpPost]
    public async Task<ActionResult<CreateOrderResponse>> Create([FromBody] CreateOrderCommand command, CancellationToken ct)
        => Ok(await _mediator.Send(command, ct));

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderListItem>>> List(
        [FromQuery] OrderStatus? status,
        CancellationToken ct)
        => Ok(await _mediator.Send(new GetOrdersQuery(status), ct));

    [HttpPut("{id:guid}/status")]
    public async Task<ActionResult<UpdateOrderStatusResponse>> UpdateStatus(
        Guid id,
        [FromBody] UpdateOrderStatusRequest body,
        CancellationToken ct)
        => Ok(await _mediator.Send(new UpdateOrderStatusCommand(id, body.Status), ct));

    [HttpGet("{id:guid}/details")]
    public async Task<ActionResult<OrderDetailsResponse>> GetDetails(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetOrderDetailsQuery(id), ct));

    public record UpdateOrderStatusRequest(OrderStatus Status);
}
