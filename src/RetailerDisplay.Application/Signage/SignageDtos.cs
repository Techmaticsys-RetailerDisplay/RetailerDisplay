namespace RetailerDisplay.Application.Signage;

/// <summary>Request body — mirrors the legacy Signage input (DeviceId + AccessKey).</summary>
public class SignageRequest
{
    public string? DeviceId { get; set; }
    public string? AccessKey { get; set; }
}

/// <summary>Top-level wrapper — matches legacy SignageResult { GetSignageItemResult }.</summary>
public class SignageResultDto
{
    public SignageDto GetSignageItemResult { get; set; } = new();
}

/// <summary>Matches the legacy Signage model field-for-field (PascalCase).</summary>
public class SignageDto
{
    public string IsAccess { get; set; } = "False";
    public string Message { get; set; } = "";
    public long ServerHitTime { get; set; }
    public int TemplateId { get; set; }
    public bool IsVertical { get; set; }
    public bool IsAntiClockwise { get; set; }
    public string? AccessKey { get; set; }
    public string? DeviceId { get; set; }
    public List<SignageItemDto> Items { get; set; } = new();
}

public class SignageItemDto
{
    public int ItemPriority { get; set; }
    public string ItemType { get; set; } = "";
    public int ItemDisplayTime { get; set; }
    /// <summary>Runtime type is one of the Signage*Item classes below (serialized by runtime type).</summary>
    public object? Item { get; set; }
}

// --- Item payloads (match legacy Image / CustomVideo / Product shapes) ---

public class SignageImageItem
{
    public string? CustomImage { get; set; }
    public string? ID { get; set; }
}

public class SignageVideoItem
{
    public string? VideoUrl { get; set; }
    public string? ID { get; set; }
}

public class SignageProductItem
{
    public string? ProductName { get; set; }
    public decimal ProductPrice { get; set; }
    public decimal ProductOfferPrice { get; set; }
    public string? ProductImage { get; set; }
    public string ProductCurrencySymbol { get; set; } = "$";
    public string? ProductDescription { get; set; }
    public string? ID { get; set; }
}
