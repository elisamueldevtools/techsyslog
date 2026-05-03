using MediatR;

namespace TechsysLog.Application.Features.Auth.Refresh;

public record RefreshTokenCommand(string RefreshToken) : IRequest<RefreshTokenResponse>;

public record RefreshTokenResponse(string AccessToken, string RefreshToken, int ExpiresIn);
