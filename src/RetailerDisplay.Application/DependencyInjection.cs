using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RetailerDisplay.Application.Auth;
using RetailerDisplay.Application.Catalog;
using RetailerDisplay.Application.Content;
using RetailerDisplay.Application.Dashboard;
using RetailerDisplay.Application.DeviceApi;
using RetailerDisplay.Application.Devices;
using RetailerDisplay.Application.Media;
using RetailerDisplay.Application.Playlists;
using RetailerDisplay.Application.Stores;

namespace RetailerDisplay.Application;

/// <summary>Registers Application-layer services and validators into DI.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IMediaService, MediaService>();
        services.AddScoped<IStoreService, StoreService>();
        services.AddScoped<IMasterCatalogService, MasterCatalogService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IContentService, ContentService>();
        services.AddScoped<IPlaylistService, PlaylistService>();
        services.AddScoped<IDeviceService, DeviceService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IDeviceApiService, DeviceApiService>();

        return services;
    }
}
