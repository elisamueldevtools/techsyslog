using MediatR;

namespace TechsysLog.Application.Features.Auth.Register;

public record RegisterCommand(string Name, string Email, string Password) : IRequest<RegisterResponse>;

public record RegisterResponse(Guid Id, string Name, string Email);
