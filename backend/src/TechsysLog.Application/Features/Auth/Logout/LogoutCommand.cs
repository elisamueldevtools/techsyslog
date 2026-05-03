using MediatR;

namespace TechsysLog.Application.Features.Auth.Logout;

public record LogoutCommand(string RefreshToken) : IRequest<Unit>;
