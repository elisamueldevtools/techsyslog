using TechsysLog.Domain.Common;
using TechsysLog.Domain.Enums;

namespace TechsysLog.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid? UserId { get; set; }
    public NotificationType Type { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool Read { get; set; }
}
