namespace RetailerDisplay.Application.Common.Security;

/// <summary>Generates 8-char alphanumeric device keys (no ambiguous characters).</summary>
public interface IDeviceKeyGenerator
{
    string Generate();
}
