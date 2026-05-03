namespace TechsysLog.Application.Common.Interfaces;

public record PasswordVerifyResult(bool Verified, bool NeedsRehash);

public interface IPasswordHasher
{
    string Hash(string password);
    PasswordVerifyResult Verify(string password, string hash);
}
