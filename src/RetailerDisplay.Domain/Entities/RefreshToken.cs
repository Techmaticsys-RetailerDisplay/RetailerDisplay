namespace RetailerDisplay.Domain.Entities;

/// <summary>A long-lived refresh token for JWT session renewal. Stored hashed.</summary>
public class RefreshToken
{
    public long TokenId { get; set; }
    public long RetailerId { get; set; }
    public string TokenHash { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public Retailer Retailer { get; set; } = null!;

    public bool IsActive => RevokedAt is null && ExpiresAt > DateTime.UtcNow;
}
