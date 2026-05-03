using MediatR;
using TechsysLog.Application.Common.Interfaces;
using TechsysLog.Domain.Entities;
using TechsysLog.Domain.Exceptions;

namespace TechsysLog.Application.Features.Auth.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponse>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;

    public RegisterCommandHandler(IUserRepository users, IPasswordHasher hasher)
    {
        _users = users;
        _hasher = hasher;
    }

    public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken ct)
    {
        var existing = await _users.GetByEmailAsync(request.Email, ct);
        if (existing is not null)
            throw new ConflictException("Email already registered.");

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = _hasher.Hash(request.Password)
        };

        await _users.AddAsync(user, ct);
        return new RegisterResponse(user.Id, user.Name, user.Email);
    }
}
