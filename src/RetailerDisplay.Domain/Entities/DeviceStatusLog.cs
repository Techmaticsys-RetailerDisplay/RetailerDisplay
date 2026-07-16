using RetailerDisplay.Domain.Enums;

namespace RetailerDisplay.Domain.Entities;

/// <summary>An online/offline transition, for the dashboard status timeline.</summary>
public class DeviceStatusLog
{
    public long LogId { get; set; }
    public long DeviceId { get; set; }
    public DeviceStatus Status { get; set; }
    public DateTime ChangedAt { get; set; }

    public Device Device { get; set; } = null!;
}
