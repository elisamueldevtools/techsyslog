using MongoDB.Driver;
using TechsysLog.Application.Common.Interfaces;
using TechsysLog.Domain.Entities;

namespace TechsysLog.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly MongoContext _ctx;

    public RefreshTokenRepository(MongoContext ctx) { _ctx = ctx; }

    public Task AddAsync(RefreshToken token, CancellationToken ct) =>
        _ctx.RefreshTokens.InsertOneAsync(token, cancellationToken: ct);

    public Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken ct) =>
        _ctx.RefreshTokens.Find(t => t.TokenHash == tokenHash).FirstOrDefaultAsync(ct)!;

    public Task UpdateAsync(RefreshToken token, CancellationToken ct) =>
        _ctx.RefreshTokens.ReplaceOneAsync(t => t.Id == token.Id, token, cancellationToken: ct);

    public async Task RevokeFamilyAsync(Guid familyId, DateTime revokedAt, CancellationToken ct)
    {
        var update = Builders<RefreshToken>.Update.Set(t => t.RevokedAt, revokedAt);
        await _ctx.RefreshTokens.UpdateManyAsync(
            t => t.FamilyId == familyId && t.RevokedAt == null,
            update,
            cancellationToken: ct);
    }
}
