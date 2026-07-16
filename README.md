# RetailerDisplay

Digital-signage platform. Retailers manage media, products and playlists; TV/tablet
devices (Android/iOS) pull and display an assigned playlist. A central Admin curates a
master product catalog.

## Stack

| Layer        | Tech                                             |
|--------------|--------------------------------------------------|
| Backend      | .NET 8 Web API                                   |
| Database     | PostgreSQL (`RetailerDisplay`) via EF Core + Npgsql |
| Media        | S3 (single bucket); MinIO for local dev          |
| Retailer web | React + Vite + TypeScript (`web/`)               |
| Devices      | Android / iOS (separate repos, consume `/device/*`) |

## Solution layout

```
src/
  RetailerDisplay.Api/             ASP.NET Core 8 Web API — controllers, DI, auth
  RetailerDisplay.Application/     services, DTOs, interfaces, validators
  RetailerDisplay.Domain/          the 12 tbl* entities + enums (zero dependencies)
  RetailerDisplay.Infrastructure/  EF Core + migrations, S3, ImageSharp, JWT
web/                               React retailer app
tests/                             unit + integration
```

## Prerequisites

- .NET SDK (pinned to 9.0.x via `global.json`; builds `net8.0`)
- Node.js 20+
- Docker (for local Postgres + MinIO)

## Getting started

```bash
# 1. Start local dependencies (Postgres + MinIO)
docker compose up -d

# 2. Apply the database schema
dotnet ef database update \
  --project src/RetailerDisplay.Infrastructure \
  --startup-project src/RetailerDisplay.Api

# 3. Run the API
dotnet run --project src/RetailerDisplay.Api

# 4. Run the web app (in another terminal)
cd web
npm install
npm run dev
```

- API health check: `GET /health`
- Swagger UI (Development): `/swagger`
- MinIO console: http://localhost:9001 (minioadmin / minioadmin)

## Migrations

```bash
# Add a migration
dotnet ef migrations add <Name> \
  --project src/RetailerDisplay.Infrastructure \
  --startup-project src/RetailerDisplay.Api \
  --output-dir Persistence/Migrations
```

## Configuration

`appsettings.json` + environment variables. Key sections: `ConnectionStrings:RetailerDisplay`,
`Jwt`, `Cors`, `S3`. In deployed environments these come from SSM. Never commit real secrets.

## Modules

- **Retailer** (`web/`, JWT auth) — media, products, playlists, devices, dashboard.
- **Device** (`/device/*`, device-key auth) — pair, fetch manifest, heartbeat.
- **Admin** (later phase) — curates the master product catalog.
