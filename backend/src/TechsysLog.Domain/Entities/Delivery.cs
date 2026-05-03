using TechsysLog.Domain.Common;

namespace TechsysLog.Domain.Entities;

public class Delivery : BaseEntity
{
    public Guid OrderId { get; set; }
    public DateTime DeliveredAt { get; set; }
    public string Notes { get; set; } = string.Empty;
}
