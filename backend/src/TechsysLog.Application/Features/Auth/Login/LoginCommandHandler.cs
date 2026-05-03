using MediatR;
using TechsysLog.Application.Common.Interfaces;
using TechsysLog.Application.Common.Security;
using TechsysLog.Domain.Entities;
using TechsysLog.Domain.Exceptions;

namespace TechsysLog.Application.Features.Auth.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _jwt;
    private readonly IRefreshTokenRepository _refresh;

    public LoginCommandHandler(
        IUserRepository users,
        IPasswordHasher hasher,
        IJwtTokenService jwt,
        IRefreshTokenRepository refresh)
    {
        _users = users;
        _hasher = hasher;
        _jwt = jwt;
        _refresh = refresh;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await _users.GetByEmailAsync(request.Email, ct)
                   ?? throw new UnauthorizedException("Invalid credentials.");

        var verify = _hasher.Verify(request.Password, user.PasswordHash);
        if (!verify.Verified)
            throw new UnauthorizedException("Invalid credentials.");

        if (verify.NeedsRehash)
        {
            user.PasswordHash = _hasher.Hash(request.Password);
            await _users.UpdateAsync(user, ct);
        }

        var refreshRaw = _jwt.GenerateRefreshTokenRaw();
        var now = DateTime.UtcNow;
        var refreshEntity = new RefreshToken
        {
            UserId = user.Id,
            FamilyId = Guid.NewGuid(),
            TokenHash = TokenHasher.Hash(refreshRaw),
            IssuedAt = now,
            ExpiresAt = now.Add(_jwt.RefreshTokenLifetime)
        };
        await _refresh.AddAsync(refreshEntity, ct);

        var tokens = _jwt.Generate(user, refreshRaw);
        return new LoginResponse(tokens.AccessToken, tokens.RefreshToken, tokens.ExpiresIn);
    }
}
