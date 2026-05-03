using TechsysLog.Domain.Entities;

namespace TechsysLog.Application.Common.Interfaces;

public interface IDeliveryRepository
{
    Task AddAsync(Delivery delivery, CancellationToken ct);
    Task<IReadOnlyList<Delivery>> GetByOrderIdAsync(Guid orderId, CancellationToken ct);
}
