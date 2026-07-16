using RetailerDisplay.Domain.Enums;

namespace RetailerDisplay.Domain.Entities;

/// <summary>A TV / tablet, claimed by its unique 8-char device key.</summary>
public class Device
{
    public long DeviceId { get; set; }
    public long RetailerId { get; set; }
    public long? StoreId { get; set; }
    /// <summary>System-generated, 8-char alphanumeric, unique. No ambiguous characters.</summary>
    public string DeviceKey { get; set; } = null!;
    public string? DeviceName { get; set; }
    /// <summary>The assigned playlist (nullable until one is assigned).</summary>
    public long? PlaylistId { get; set; }

    // Reported by the device on pair / heartbeat
    public Orientation? Orientation { get; set; }
    public int? ScreenWidth { get; set; }
    public int? ScreenHeight { get; set; }
    public string? AppVersion { get; set; }

    /// <summary>Heartbeat timestamp — drives online/offline status.</summary>
    public DateTime? LastSeenAt { get; set; }
    /// <summary>Playlist version the device currently holds.</summary>
    public int? LastSyncedVersion { get; set; }
    /// <summary>Remote-refresh flag; the device clears it on next sync.</summary>
    public bool RefreshRequested { get; set; }
    public bool IsRevoked { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Retailer Retailer { get; set; } = null!;
    public Store? Store { get; set; }
    public Playlist? Playlist { get; set; }
    public ICollection<DeviceStatusLog> StatusLogs { get; set; } = new List<DeviceStatusLog>();
}
