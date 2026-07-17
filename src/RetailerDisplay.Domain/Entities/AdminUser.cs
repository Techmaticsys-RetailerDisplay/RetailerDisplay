namespace RetailerDisplay.Domain.Entities;

/// <summary>A Bottlecapps super-admin — manages the master catalog and oversees retailers.</summary>
public class AdminUser
{
    public long AdminUserId { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? Name { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
