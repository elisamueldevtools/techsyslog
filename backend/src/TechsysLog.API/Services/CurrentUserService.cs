using System.Security.Claims;
using TechsysLog.Application.Common.Interfaces;

namespace TechsysLog.API.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUserService(IHttpContextAccessor accessor) { _accessor = accessor; }

    public Guid? UserId
    {
        get
        {
            var sub = _accessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                       ?? _accessor.HttpContext?.User?.FindFirstValue("sub");
            return Guid.TryParse(sub, out var id) ? id : null;
        }
    }

    public string? Role => _accessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role)
                            ?? _accessor.HttpContext?.User?.FindFirstValue("role");

    public bool IsAuthenticated => _accessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
