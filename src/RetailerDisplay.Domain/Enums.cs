namespace RetailerDisplay.Domain.Enums;

/// <summary>The kind of thing a Content item displays.</summary>
public enum ContentType : short
{
    Image = 1,
    Video = 2,
    ProductList = 3
}

/// <summary>How a media item fills the screen when aspect ratios differ.</summary>
public enum FitMode : short
{
    /// <summary>Whole image, letterboxed if needed. Default.</summary>
    Contain = 1,
    /// <summary>Fill the screen, cropping overflow.</summary>
    Cover = 2,
    /// <summary>Distort to fit.</summary>
    Stretch = 3
}

/// <summary>Physical screen orientation reported by a device.</summary>
public enum Orientation : short
{
    Landscape = 1,
    Portrait = 2
}

/// <summary>Where a store product originated.</summary>
public enum ProductSource : short
{
    Master = 1,
    Csv = 2,
    Manual = 3
}

/// <summary>Device connectivity state, derived from heartbeat.</summary>
public enum DeviceStatus : short
{
    Online = 1,
    Offline = 2
}

/// <summary>Lifecycle of a CSV product-import batch.</summary>
public enum ImportStatus : short
{
    Pending = 1,
    Completed = 2,
    Failed = 3
}
