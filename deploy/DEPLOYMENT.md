# RetailerDisplay — Deployment guide (public host)

Three things run: the **API** (.NET 8), the **React web app** (static), and **PostgreSQL**.
Uploaded media is stored on the server's disk for now (switch to S3 later — config only).

## 1. Database (PostgreSQL)
1. Create an empty database named **`RetailerDisplay`**.
2. Run **`deploy/schema.sql`** against it (creates all tables; idempotent — safe to re-run):
   ```
   psql "host=... dbname=RetailerDisplay user=... password=..." -f schema.sql
   ```
   No .NET SDK needed on the server for this.

## 2. API (.NET 8)
- Publish: `dotnet publish src/RetailerDisplay.Api -c Release -o out`
- Host it behind HTTPS (nginx / IIS / container). It listens on the port you configure.
- Provide configuration via **`appsettings.Production.json`** (see `appsettings.Production.template.json`) **or environment variables**:
  - `ConnectionStrings__RetailerDisplay`  → the Postgres connection string
  - `Jwt__Key`                            → a long random secret
  - `Media__PublicBaseUrl`                → the API's public https URL (used in media links)
  - `Media__LocalRoot`                    → a persistent folder for uploads (e.g. `/var/retailerdisplay/media`)
  - `Cors__WebOrigin`                     → the web app's public URL
- **Media folder must be persistent** (a mounted volume if containerized) or uploads are lost on redeploy.

## 3. Web app (React)
- Build with the API URL: `VITE_API_BASE_URL=https://api.YOURDOMAIN.com/api/v1 npm run build` (in `web/`)
- Serve the resulting `web/dist` as static files (nginx / any static host).

## 4. First admin account
The dev auto-seed only runs in Development. In Production, create the first admin once
(ask the dev team to run the seeder with credentials, or insert into `tblAdminUser`
with a bcrypt password hash). Then admins create retailers from the admin console.

## 5. Device (TV/tablet) integration
The app calls **`POST https://api.YOURDOMAIN.com/api/v1/signage/GetSignageItemResult`**
with body `{ "AccessKey": "<device key>", "DeviceId": "<device hardware id, optional>" }`.
Response is the legacy `GetSignageItemResult` format.

## What to hand to whoever deploys
- This repo (or the published API build + `web/dist`)
- `deploy/schema.sql`
- The production config values (connection string, JWT secret, public URLs, media path)
- A PostgreSQL instance + an empty `RetailerDisplay` database
