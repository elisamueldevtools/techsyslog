using MediatR;

namespace TechsysLog.Application.Features.Auth.Login;

public record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;

public record LoginResponse(string AccessToken, string RefreshToken, int ExpiresIn);
