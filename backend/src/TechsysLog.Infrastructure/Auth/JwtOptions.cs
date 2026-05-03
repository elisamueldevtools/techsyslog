namespace TechsysLog.Infrastructure.Auth;

public class JwtOptions
{
    public string Issuer { get; set; } = "TechsysLog";
    public string Audience { get; set; } = "TechsysLog";
    public string Key { get; set; } = string.Empty;
    public int AccessTokenMinutes { get; set; } = 15;
    public int RefreshTokenDays { get; set; } = 7;
}
