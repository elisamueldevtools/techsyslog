namespace TechsysLog.Infrastructure.Auth;

public class PasswordOptions
{
    /// <summary>Base64 da chave de 64 bytes usada como pepper (HMAC-SHA512).</summary>
    public string Pepper { get; set; } = string.Empty;
}
