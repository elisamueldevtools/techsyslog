using MediatR;
using TechsysLog.Application.Common.Interfaces;
using TechsysLog.Application.Common.Security;

namespace TechsysLog.Application.Features.Auth.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
{
    private readonly IRefreshTokenRepository _refresh;

    public LogoutCommandHandler(IRefreshTokenRepository refresh) { _refresh = refresh; }

    public async Task<Unit> Handle(LogoutCommand request, CancellationToken ct)
    {
        var hash = TokenHasher.Hash(request.RefreshToken);
        var stored = await _refresh.GetByHashAsync(hash, ct);
        if (stored is not null && stored.RevokedAt is null)
        {
            stored.RevokedAt = DateTime.UtcNow;
            await _refresh.UpdateAsync(stored, ct);
        }
        return Unit.Value;
    }
}
