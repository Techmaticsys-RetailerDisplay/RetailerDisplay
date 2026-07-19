using Microsoft.EntityFrameworkCore;
using RetailerDisplay.Application.Common.Security;
using RetailerDisplay.Domain.Entities;

namespace RetailerDisplay.Infrastructure.Persistence;

public static class DbSeeder
{
    /// <summary>
    /// Creates the default admin if it doesn't exist.
    /// If it already exists, updates its password and ensures it is active.
    /// </summary>
    public static async Task EnsureDefaultAdminAsync(
        RetailerDisplayDbContext db,
        IPasswordHasher hasher,
        string email,
        string password)
    {
        var normalized = email.Trim().ToLowerInvariant();

        var admin = await db.AdminUsers
            .FirstOrDefaultAsync(a => a.Email == normalized);

        var now = DateTime.UtcNow;

        if (admin == null)
        {
            db.AdminUsers.Add(new AdminUser
            {
                Email = normalized,
                PasswordHash = hasher.Hash(password),
                Name = "Bottlecapps Admin",
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            });
        }
        else
        {
            // Update existing admin
            admin.PasswordHash = hasher.Hash(password);
            admin.IsActive = true;
            admin.UpdatedAt = now;
        }

        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Ensures every retailer has at least one store
    /// (backfills a default named after the business).
    /// </summary>
    public static async Task EnsureDefaultStoresAsync(RetailerDisplayDbContext db)
    {
        var storeless = await db.Retailers
            .Where(r => !db.Stores.Any(s => s.RetailerId == r.RetailerId))
            .ToListAsync();

        if (storeless.Count == 0)
            return;

        var now = DateTime.UtcNow;

        foreach (var r in storeless)
        {
            db.Stores.Add(new Store
            {
                RetailerId = r.RetailerId,
                StoreName = r.BusinessName,
                TimeZone = "UTC",
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        await db.SaveChangesAsync();
    }
}