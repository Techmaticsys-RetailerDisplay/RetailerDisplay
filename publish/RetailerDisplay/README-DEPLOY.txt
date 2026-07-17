RetailerDisplay — Deployment (Windows server, self-contained)
=============================================================

This folder is a self-contained build. The .NET runtime is bundled — nothing
to install except PostgreSQL.

------------------------------------------------------------
1) PostgreSQL (one-time)
------------------------------------------------------------
- Install PostgreSQL 14+ on the server (or point at an existing one).
- Create an empty database named exactly:  RetailerDisplay
    Example (psql):  CREATE DATABASE "RetailerDisplay";
- Note the host, port, username, password.
  (You do NOT need to create tables — the app creates them automatically on
   first run. A db\schema.sql is included if you prefer to create them manually.)

------------------------------------------------------------
2) Configure  (edit appsettings.Production.json — next to the .exe)
------------------------------------------------------------
- ConnectionStrings:RetailerDisplay  -> your Postgres host/user/password
- Jwt:Key                            -> any long random string (32+ chars)
- Media:PublicBaseUrl                -> http://<SERVER_IP>:8080   (the IP + port
                                        devices/browsers use to reach this server)
- Cors:WebOrigin                     -> same as PublicBaseUrl
- SeedAdmin:Email / Password         -> the first admin login (used at /admin)
- Urls                               -> http://0.0.0.0:8080  (change port if needed)

------------------------------------------------------------
3) Run
------------------------------------------------------------
- Double-click  RetailerDisplay.Api.exe   (or run it from a terminal).
- On first run it creates all tables and the admin account, then starts listening.
- Open a firewall rule for the port (default 8080) so other devices can reach it.

  To run it permanently as a Windows Service (recommended):
      sc create RetailerDisplay binPath= "C:\path\to\RetailerDisplay.Api.exe" start= auto
      sc start RetailerDisplay

------------------------------------------------------------
4) Access  (replace SERVER_IP and port)
------------------------------------------------------------
- Retailer / web app:   http://SERVER_IP:8080
- Admin console:        http://SERVER_IP:8080/admin
- Health check:         http://SERVER_IP:8080/health
- Device (TV) API:      POST http://SERVER_IP:8080/api/v1/signage/GetSignageItemResult
                        body: { "AccessKey": "<device key>", "DeviceId": "<optional>" }

------------------------------------------------------------
Notes
------------------------------------------------------------
- Uploaded images/videos are stored in the  media\  folder next to the exe.
  KEEP that folder when updating the app, or uploads are lost.
- To update: stop the app, replace the files EXCEPT appsettings.Production.json
  and the media\ folder, start again.
