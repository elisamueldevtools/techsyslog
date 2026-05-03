using MongoDB.Driver;
using TechsysLog.Application.Common.Interfaces;
using TechsysLog.Domain.Entities;

namespace TechsysLog.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MongoContext _ctx;

    public UserRepository(MongoContext ctx) { _ctx = ctx; }

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct) =>
        _ctx.Users.Find(u => u.Email == email).FirstOrDefaultAsync(ct)!;

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _ctx.Users.Find(u => u.Id == id).FirstOrDefaultAsync(ct)!;

    public Task AddAsync(User user, CancellationToken ct) =>
        _ctx.Users.InsertOneAsync(user, cancellationToken: ct);

    public Task UpdateAsync(User user, CancellationToken ct) =>
        _ctx.Users.ReplaceOneAsync(u => u.Id == user.Id, user, cancellationToken: ct);
}
