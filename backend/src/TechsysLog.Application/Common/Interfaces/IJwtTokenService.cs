using TechsysLog.Domain.Entities;

namespace TechsysLog.Application.Common.Interfaces;

public record AuthTokens(string AccessToken, string RefreshToken, int ExpiresIn);

public interface IJwtTokenService
{
    AuthTokens Generate(User user, string refreshTokenRaw);
    string GenerateRefreshTokenRaw();
    TimeSpan RefreshTokenLifetime { get; }
}
