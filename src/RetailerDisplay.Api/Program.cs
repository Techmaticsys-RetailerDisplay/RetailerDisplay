using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RetailerDisplay.Api.Common;
using RetailerDisplay.Application;
using RetailerDisplay.Application.Common;
using RetailerDisplay.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// --- Services ---
builder.Services.AddControllers(options => options.Filters.Add<ValidationFilter>());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

// CORS for the React retailer app
var webOrigin = builder.Configuration["Cors:WebOrigin"] ?? "http://localhost:5173";
builder.Services.AddCors(options =>
    options.AddPolicy("web", policy => policy
        .WithOrigins(webOrigin)
        .AllowAnyHeader()
        .AllowAnyMethod()));

// JWT authentication (retailer). Device endpoints use a separate key scheme.
var jwtKey = builder.Configuration["Jwt:Key"];
if (!string.IsNullOrWhiteSpace(jwtKey))
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };
        });
}
builder.Services.AddAuthorization();

var app = builder.Build();

app_configured(app);

// --- Migrate DB + seed admin on startup (idempotent) ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RetailerDisplay.Infrastructure.Persistence.RetailerDisplayDbContext>();
    db.Database.Migrate();

    var hasher = scope.ServiceProvider.GetRequiredService<RetailerDisplay.Application.Common.Security.IPasswordHasher>();

    if (app.Environment.IsDevelopment())
    {
        await RetailerDisplay.Infrastructure.Persistence.DbSeeder.EnsureDefaultAdminAsync(db, hasher, "admin@bottlecapps.com", "Admin#2026!");
        await RetailerDisplay.Infrastructure.Persistence.DbSeeder.EnsureDefaultAdminAsync(db, hasher, "anand.ch2@gmail.com", "Anand#2026!");
    }

    // Production: seed the first admin from configuration (SeedAdmin section or env var) if provided.
    var seedEmail = app.Configuration["SeedAdmin:Email"];
    var seedPassword = app.Configuration["SeedAdmin:Password"];
    if (!string.IsNullOrWhiteSpace(seedEmail) && !string.IsNullOrWhiteSpace(seedPassword))
        await RetailerDisplay.Infrastructure.Persistence.DbSeeder.EnsureDefaultAdminAsync(db, hasher, seedEmail, seedPassword);

    await RetailerDisplay.Infrastructure.Persistence.DbSeeder.EnsureDefaultStoresAsync(db);
}

app.Run();

// Configures the middleware pipeline (kept as a local function so startup reads top-to-bottom).
static WebApplication app_configured(WebApplication app)
{
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.UseSwagger();
    app.UseSwaggerUI();

    // Serve the built React app (copied into wwwroot in the container).
    app.UseDefaultFiles();
    app.UseStaticFiles();

    app.UseCors("web");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "RetailerDisplay.Api" })).WithName("Health");

    // SPA fallback: client-side routes (/login, /admin, …) return index.html.
    app.MapFallbackToFile("index.html");

    return app;
}
