using Amazon.Runtime;
using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RetailerDisplay.Application.Common;
using RetailerDisplay.Application.Common.Imaging;
using RetailerDisplay.Application.Common.Security;
using RetailerDisplay.Application.Common.Storage;
using RetailerDisplay.Infrastructure.Imaging;
using RetailerDisplay.Infrastructure.Persistence;
using RetailerDisplay.Infrastructure.Security;
using RetailerDisplay.Infrastructure.Storage;

namespace RetailerDisplay.Infrastructure;

/// <summary>Registers infrastructure services (EF Core, storage, imaging, security) into DI.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("RetailerDisplay");

        services.AddDbContext<RetailerDisplayDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsAssembly(typeof(RetailerDisplayDbContext).Assembly.FullName)));

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<RetailerDisplayDbContext>());

        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IDeviceKeyGenerator, DeviceKeyGenerator>();

        // Object storage (S3 / MinIO)
        services.Configure<S3Options>(configuration.GetSection("S3"));
        services.AddSingleton<IAmazonS3>(sp =>
        {
            var o = sp.GetRequiredService<IOptions<S3Options>>().Value;
            var config = new AmazonS3Config { ForcePathStyle = o.ForcePathStyle };
            if (!string.IsNullOrWhiteSpace(o.ServiceUrl))
                config.ServiceURL = o.ServiceUrl;
            else
                config.RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(o.Region);

            return !string.IsNullOrWhiteSpace(o.AccessKey)
                ? new AmazonS3Client(new BasicAWSCredentials(o.AccessKey, o.SecretKey), config)
                : new AmazonS3Client(config);
        });
        services.AddScoped<IMediaStorage, S3MediaStorage>();
        services.AddSingleton<IImageProcessor, ImageSharpImageProcessor>();

        services.AddSingleton<Application.Catalog.ICsvProductImporter, Catalog.CsvProductImporter>();

        return services;
    }
}
