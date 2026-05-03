using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechsysLog.Application.Features.Cep.LookupCep;

namespace TechsysLog.API.Controllers;

[ApiController]
[Authorize]
[Route("cep")]
public class CepController : ControllerBase
{
    private readonly IMediator _mediator;

    public CepController(IMediator mediator) { _mediator = mediator; }

    [HttpGet("{cep}")]
    public async Task<ActionResult<LookupCepResponse>> Lookup(string cep, CancellationToken ct)
        => Ok(await _mediator.Send(new LookupCepQuery(cep), ct));
}
