using MediatR;
using Microsoft.AspNetCore.Mvc;
using TechsysLog.Application.Features.Auth.Login;
using TechsysLog.Application.Features.Auth.Logout;
using TechsysLog.Application.Features.Auth.Refresh;
using TechsysLog.Application.Features.Auth.Register;

namespace TechsysLog.API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator) { _mediator = mediator; }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterCommand command, CancellationToken ct)
        => Ok(await _mediator.Send(command, ct));

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginCommand command, CancellationToken ct)
        => Ok(await _mediator.Send(command, ct));

    [HttpPost("refresh")]
    public async Task<ActionResult<RefreshTokenResponse>> Refresh([FromBody] RefreshTokenCommand command, CancellationToken ct)
        => Ok(await _mediator.Send(command, ct));

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutCommand command, CancellationToken ct)
    {
        await _mediator.Send(command, ct);
        return NoContent();
    }
}
