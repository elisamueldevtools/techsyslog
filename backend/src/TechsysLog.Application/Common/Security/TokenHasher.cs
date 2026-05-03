using System.Security.Cryptography;
using System.Text;

namespace TechsysLog.Application.Common.Security;

public static class TokenHasher
{
    public static string Hash(string tokenRaw)
    {
        var bytes = Encoding.UTF8.GetBytes(tokenRaw);
        return Convert.ToHexString(SHA256.HashData(bytes));
    }
}
