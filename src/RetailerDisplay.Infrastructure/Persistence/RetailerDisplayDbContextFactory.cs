using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RetailerDisplay.Infrastructure.Persistence;

/// <summary>
/// Design-time factory so EF Core tooling (migrations) can build the context
/// without booting the API. Uses RETAILERDISPLAY_DB env var, else a local default.
/// </summary>
public class RetailerDisplayDbContextFactory : IDesignTimeDbContextFactory<RetailerDisplayDbContext>
{
    public RetailerDisplayDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("RETAILERDISPLAY_DB")
            ?? "Host=localhost;Port=5432;Database=RetailerDisplay;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<RetailerDisplayDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new RetailerDisplayDbContext(options);
    }
}
