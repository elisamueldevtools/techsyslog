using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using TechsysLog.Application.Common.Interfaces;

namespace TechsysLog.Infrastructure.Auth;

public class PepperedBCryptPasswordHasher : IPasswordHasher
{
    private const string PepperPrefix = "p1$";
    private readonly byte[] _pepper;

    public PepperedBCryptPasswordHasher(IOptions<PasswordOptions> opts)
    {
        var raw = opts.Value.Pepper ?? string.Empty;
        byte[] decoded;
        try
        {
            decoded = Convert.FromBase64String(raw);
        }
        catch (FormatException)
        {
            throw new InvalidOperationException("Password.Pepper deve ser uma string base64 válida.");
        }

        if (decoded.Length != 64)
            throw new InvalidOperationException(
                $"Password.Pepper deve ter exatamente 64 bytes após decodificação base64 (recebeu {decoded.Length}).");

        _pepper = decoded;
    }

    public string Hash(string password)
    {
        var input = ApplyPepper(password);
        var bcrypt = BCrypt.Net.BCrypt.HashPassword(input);
        return PepperPrefix + bcrypt;
    }

    public PasswordVerifyResult Verify(string password, string hash)
    {
        if (hash.StartsWith(PepperPrefix, StringComparison.Ordinal))
        {
            var stripped = hash[PepperPrefix.Length..];
            var input = ApplyPepper(password);
            var ok = BCrypt.Net.BCrypt.Verify(input, stripped);
            return new PasswordVerifyResult(ok, NeedsRehash: false);
        }

        var legacyOk = BCrypt.Net.BCrypt.Verify(password, hash);
        return new PasswordVerifyResult(legacyOk, NeedsRehash: legacyOk);
    }

    private string ApplyPepper(string password)
    {
        using var hmac = new HMACSHA512(_pepper);
        var bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
