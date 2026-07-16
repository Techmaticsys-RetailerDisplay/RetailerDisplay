namespace RetailerDisplay.Application.Devices;

/// <summary>Shared rule for deriving online/offline from a device's last heartbeat.</summary>
public static class DeviceStatusRules
{
    /// <summary>A device is Online if it was seen within this window.</summary>
    public static readonly TimeSpan OnlineWindow = TimeSpan.FromSeconds(90);

    public static bool IsOnline(DateTime? lastSeenAt) =>
        lastSeenAt.HasValue && (DateTime.UtcNow - lastSeenAt.Value) < OnlineWindow;

    public static string StatusText(DateTime? lastSeenAt) => IsOnline(lastSeenAt) ? "Online" : "Offline";
}
