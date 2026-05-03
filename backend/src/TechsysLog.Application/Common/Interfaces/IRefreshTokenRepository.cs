using TechsysLog.Domain.Entities;

namespace TechsysLog.Application.Common.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token, CancellationToken ct);
    Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken ct);
    Task UpdateAsync(RefreshToken token, CancellationToken ct);
    Task RevokeFamilyAsync(Guid familyId, DateTime revokedAt, CancellationToken ct);
}
