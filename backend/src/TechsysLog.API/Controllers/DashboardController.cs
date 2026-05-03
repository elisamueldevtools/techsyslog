using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechsysLog.Application.Features.Dashboard.GetDashboard;

namespace TechsysLog.API.Controllers;

[ApiController]
[Authorize]
[Route("dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator) { _mediator = mediator; }

    [HttpGet]
    public async Task<ActionResult<DashboardResponse>> Get(
        [FromQuery] int month,
        [FromQuery] int year,
        CancellationToken ct)
        => Ok(await _mediator.Send(new GetDashboardQuery(month, year), ct));
}
