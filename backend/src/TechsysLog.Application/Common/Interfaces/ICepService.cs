using TechsysLog.Domain.Entities;

namespace TechsysLog.Application.Common.Interfaces;

public interface ICepService
{
    Task<Address?> LookupAsync(string cep, CancellationToken ct);
}
