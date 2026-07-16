using RetailerDisplay.Application.Common.Security;

namespace RetailerDisplay.Infrastructure.Security;

/// <summary>BCrypt-based password hashing.</summary>
public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

    public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
}
