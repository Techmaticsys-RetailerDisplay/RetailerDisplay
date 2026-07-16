namespace RetailerDisplay.Application.Common.Security;

/// <summary>Hashes and verifies retailer passwords. Implemented with BCrypt in Infrastructure.</summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
