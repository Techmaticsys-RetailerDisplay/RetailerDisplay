using System.Security.Cryptography;
using RetailerDisplay.Application.Common.Security;

namespace RetailerDisplay.Infrastructure.Security;

public class DeviceKeyGenerator : IDeviceKeyGenerator
{
    // Excludes ambiguous characters (0/O, 1/I/L) so keys are easy to read off a screen.
    private const string Alphabet = "ABCDEFGHJKMNPQRSTUVWXYZ23456789";
    private const int Length = 8;

    public string Generate()
    {
        Span<char> chars = stackalloc char[Length];
        for (var i = 0; i < Length; i++)
        {
            var idx = RandomNumberGenerator.GetInt32(Alphabet.Length);
            chars[i] = Alphabet[idx];
        }
        return new string(chars);
    }
}
