using MediatR;
using TechsysLog.Application.Common.Interfaces;
using TechsysLog.Application.Common.Security;
using TechsysLog.Domain.Entities;
using TechsysLog.Domain.Exceptions;

namespace TechsysLog.Application.Features.Auth.Refresh;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly IRefreshTokenRepository _refresh;
    private readonly IUserRepository _users;
    private readonly IJwtTokenService _jwt;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refresh,
        IUserRepository users,
        IJwtTokenService jwt)
    {
        _refresh = refresh;
        _users = users;
        _jwt = jwt;
    }

    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var hash = TokenHasher.Hash(request.RefreshToken);
        var stored = await _refresh.GetByHashAsync(hash, ct)
                     ?? throw new UnauthorizedException("Invalid refresh token.");

        var now = DateTime.UtcNow;

        if (stored.RevokedAt is not null)
        {
            await _refresh.RevokeFamilyAsync(stored.FamilyId, now, ct);
            throw new UnauthorizedException("Refresh token reuse detected.");
        }

        if (stored.ExpiresAt <= now)
            throw new UnauthorizedException("Refresh token expired.");

        var user = await _users.GetByIdAsync(stored.UserId, ct)
                   ?? throw new UnauthorizedException("User not found.");

        var newRaw = _jwt.GenerateRefreshTokenRaw();
        var newToken = new RefreshToken
        {
            UserId = user.Id,
            FamilyId = stored.FamilyId,
            TokenHash = TokenHasher.Hash(newRaw),
            IssuedAt = now,
            ExpiresAt = now.Add(_jwt.RefreshTokenLifetime)
        };
        await _refresh.AddAsync(newToken, ct);

        stored.RevokedAt = now;
        stored.ReplacedById = newToken.Id;
        await _refresh.UpdateAsync(stored, ct);

        var tokens = _jwt.Generate(user, newRaw);
        return new RefreshTokenResponse(tokens.AccessToken, tokens.RefreshToken, tokens.ExpiresIn);
    }
}
