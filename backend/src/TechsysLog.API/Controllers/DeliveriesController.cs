using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechsysLog.Application.Features.Deliveries.CreateDelivery;

namespace TechsysLog.API.Controllers;

[ApiController]
[Authorize]
[Route("deliveries")]
public class DeliveriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public DeliveriesController(IMediator mediator) { _mediator = mediator; }

    [HttpPost]
    public async Task<ActionResult<CreateDeliveryResponse>> Create(
        [FromBody] CreateDeliveryCommand command,
        CancellationToken ct)
        => Ok(await _mediator.Send(command, ct));
}
