using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RetailerDisplay.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblMasterProduct",
                columns: table => new
                {
                    MasterProductId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Sku = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Upc = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ProductName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Category = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Brand = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ProductType = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    Abv = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: true),
                    ContainerType = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    Volume = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PackSize = table.Column<int>(type: "integer", nullable: true),
                    Vintage = table.Column<int>(type: "integer", nullable: true),
                    DefaultImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblMasterProduct", x => x.MasterProductId);
                });

            migrationBuilder.CreateTable(
                name: "tblRetailer",
                columns: table => new
                {
                    RetailerId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    BusinessName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ContactName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Phone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblRetailer", x => x.RetailerId);
                });

            migrationBuilder.CreateTable(
                name: "tblContent",
                columns: table => new
                {
                    ContentId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RetailerId = table.Column<long>(type: "bigint", nullable: false),
                    ContentType = table.Column<short>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    MasterKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    MediaVariants = table.Column<string>(type: "jsonb", nullable: true),
                    ThumbnailKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DurationSeconds = table.Column<int>(type: "integer", nullable: true),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    ContentHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblContent", x => x.ContentId);
                    table.ForeignKey(
                        name: "FK_tblContent_tblRetailer_RetailerId",
                        column: x => x.RetailerId,
                        principalTable: "tblRetailer",
                        principalColumn: "RetailerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblPlaylist",
                columns: table => new
                {
                    PlaylistId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RetailerId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblPlaylist", x => x.PlaylistId);
                    table.ForeignKey(
                        name: "FK_tblPlaylist_tblRetailer_RetailerId",
                        column: x => x.RetailerId,
                        principalTable: "tblRetailer",
                        principalColumn: "RetailerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblProductImport",
                columns: table => new
                {
                    ImportId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RetailerId = table.Column<long>(type: "bigint", nullable: false),
                    StoreId = table.Column<long>(type: "bigint", nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    TotalRows = table.Column<int>(type: "integer", nullable: false),
                    SuccessCount = table.Column<int>(type: "integer", nullable: false),
                    FailCount = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    ErrorReportUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblProductImport", x => x.ImportId);
                    table.ForeignKey(
                        name: "FK_tblProductImport_tblRetailer_RetailerId",
                        column: x => x.RetailerId,
                        principalTable: "tblRetailer",
                        principalColumn: "RetailerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblRefreshToken",
                columns: table => new
                {
                    TokenId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RetailerId = table.Column<long>(type: "bigint", nullable: false),
                    TokenHash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblRefreshToken", x => x.TokenId);
                    table.ForeignKey(
                        name: "FK_tblRefreshToken_tblRetailer_RetailerId",
                        column: x => x.RetailerId,
                        principalTable: "tblRetailer",
                        principalColumn: "RetailerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblStore",
                columns: table => new
                {
                    StoreId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RetailerId = table.Column<long>(type: "bigint", nullable: false),
                    StoreName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    StoreCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AddressLine1 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    AddressLine2 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PostalCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    Longitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: true),
                    Phone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    TimeZone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblStore", x => x.StoreId);
                    table.ForeignKey(
                        name: "FK_tblStore_tblRetailer_RetailerId",
                        column: x => x.RetailerId,
                        principalTable: "tblRetailer",
                        principalColumn: "RetailerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblPlaylistItem",
                columns: table => new
                {
                    PlaylistItemId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlaylistId = table.Column<long>(type: "bigint", nullable: false),
                    ContentId = table.Column<long>(type: "bigint", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    DurationSeconds = table.Column<int>(type: "integer", nullable: true),
                    FitMode = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblPlaylistItem", x => x.PlaylistItemId);
                    table.ForeignKey(
                        name: "FK_tblPlaylistItem_tblContent_ContentId",
                        column: x => x.ContentId,
                        principalTable: "tblContent",
                        principalColumn: "ContentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tblPlaylistItem_tblPlaylist_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "tblPlaylist",
                        principalColumn: "PlaylistId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblDevice",
                columns: table => new
                {
                    DeviceId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RetailerId = table.Column<long>(type: "bigint", nullable: false),
                    StoreId = table.Column<long>(type: "bigint", nullable: true),
                    DeviceKey = table.Column<string>(type: "character(8)", fixedLength: true, maxLength: 8, nullable: false),
                    DeviceName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    PlaylistId = table.Column<long>(type: "bigint", nullable: true),
                    Orientation = table.Column<short>(type: "smallint", nullable: true),
                    ScreenWidth = table.Column<int>(type: "integer", nullable: true),
                    ScreenHeight = table.Column<int>(type: "integer", nullable: true),
                    AppVersion = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    LastSeenAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastSyncedVersion = table.Column<int>(type: "integer", nullable: true),
                    RefreshRequested = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblDevice", x => x.DeviceId);
                    table.ForeignKey(
                        name: "FK_tblDevice_tblPlaylist_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "tblPlaylist",
                        principalColumn: "PlaylistId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_tblDevice_tblRetailer_RetailerId",
                        column: x => x.RetailerId,
                        principalTable: "tblRetailer",
                        principalColumn: "RetailerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tblDevice_tblStore_StoreId",
                        column: x => x.StoreId,
                        principalTable: "tblStore",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "tblStoreProduct",
                columns: table => new
                {
                    StoreProductId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RetailerId = table.Column<long>(type: "bigint", nullable: false),
                    StoreId = table.Column<long>(type: "bigint", nullable: false),
                    MasterProductId = table.Column<long>(type: "bigint", nullable: true),
                    Source = table.Column<short>(type: "smallint", nullable: false),
                    Sku = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    ProductName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Category = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Brand = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ProductType = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    Abv = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: true),
                    ContainerType = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    Volume = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PackSize = table.Column<int>(type: "integer", nullable: true),
                    Vintage = table.Column<int>(type: "integer", nullable: true),
                    Price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    SalePrice = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    Currency = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false, defaultValue: "USD"),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    ImportBatchId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblStoreProduct", x => x.StoreProductId);
                    table.ForeignKey(
                        name: "FK_tblStoreProduct_tblMasterProduct_MasterProductId",
                        column: x => x.MasterProductId,
                        principalTable: "tblMasterProduct",
                        principalColumn: "MasterProductId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_tblStoreProduct_tblProductImport_ImportBatchId",
                        column: x => x.ImportBatchId,
                        principalTable: "tblProductImport",
                        principalColumn: "ImportId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_tblStoreProduct_tblRetailer_RetailerId",
                        column: x => x.RetailerId,
                        principalTable: "tblRetailer",
                        principalColumn: "RetailerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tblStoreProduct_tblStore_StoreId",
                        column: x => x.StoreId,
                        principalTable: "tblStore",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblDeviceStatusLog",
                columns: table => new
                {
                    LogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeviceId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblDeviceStatusLog", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_tblDeviceStatusLog_tblDevice_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "tblDevice",
                        principalColumn: "DeviceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblContentProduct",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContentId = table.Column<long>(type: "bigint", nullable: false),
                    StoreProductId = table.Column<long>(type: "bigint", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblContentProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblContentProduct_tblContent_ContentId",
                        column: x => x.ContentId,
                        principalTable: "tblContent",
                        principalColumn: "ContentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tblContentProduct_tblStoreProduct_StoreProductId",
                        column: x => x.StoreProductId,
                        principalTable: "tblStoreProduct",
                        principalColumn: "StoreProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblContent_RetailerId",
                table: "tblContent",
                column: "RetailerId");

            migrationBuilder.CreateIndex(
                name: "IX_tblContentProduct_ContentId_StoreProductId",
                table: "tblContentProduct",
                columns: new[] { "ContentId", "StoreProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblContentProduct_StoreProductId",
                table: "tblContentProduct",
                column: "StoreProductId");

            migrationBuilder.CreateIndex(
                name: "IX_tblDevice_DeviceKey",
                table: "tblDevice",
                column: "DeviceKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblDevice_PlaylistId",
                table: "tblDevice",
                column: "PlaylistId");

            migrationBuilder.CreateIndex(
                name: "IX_tblDevice_RetailerId",
                table: "tblDevice",
                column: "RetailerId");

            migrationBuilder.CreateIndex(
                name: "IX_tblDevice_StoreId",
                table: "tblDevice",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_tblDeviceStatusLog_DeviceId_ChangedAt",
                table: "tblDeviceStatusLog",
                columns: new[] { "DeviceId", "ChangedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_tblMasterProduct_Sku",
                table: "tblMasterProduct",
                column: "Sku",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblMasterProduct_Upc",
                table: "tblMasterProduct",
                column: "Upc",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblPlaylist_RetailerId",
                table: "tblPlaylist",
                column: "RetailerId");

            migrationBuilder.CreateIndex(
                name: "IX_tblPlaylistItem_ContentId",
                table: "tblPlaylistItem",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_tblPlaylistItem_PlaylistId",
                table: "tblPlaylistItem",
                column: "PlaylistId");

            migrationBuilder.CreateIndex(
                name: "IX_tblProductImport_RetailerId",
                table: "tblProductImport",
                column: "RetailerId");

            migrationBuilder.CreateIndex(
                name: "IX_tblRefreshToken_RetailerId",
                table: "tblRefreshToken",
                column: "RetailerId");

            migrationBuilder.CreateIndex(
                name: "IX_tblRefreshToken_TokenHash",
                table: "tblRefreshToken",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblRetailer_Email",
                table: "tblRetailer",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblStore_RetailerId_StoreCode",
                table: "tblStore",
                columns: new[] { "RetailerId", "StoreCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblStoreProduct_ImportBatchId",
                table: "tblStoreProduct",
                column: "ImportBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_tblStoreProduct_MasterProductId",
                table: "tblStoreProduct",
                column: "MasterProductId");

            migrationBuilder.CreateIndex(
                name: "IX_tblStoreProduct_RetailerId",
                table: "tblStoreProduct",
                column: "RetailerId");

            migrationBuilder.CreateIndex(
                name: "IX_tblStoreProduct_StoreId_Sku",
                table: "tblStoreProduct",
                columns: new[] { "StoreId", "Sku" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblContentProduct");

            migrationBuilder.DropTable(
                name: "tblDeviceStatusLog");

            migrationBuilder.DropTable(
                name: "tblPlaylistItem");

            migrationBuilder.DropTable(
                name: "tblRefreshToken");

            migrationBuilder.DropTable(
                name: "tblStoreProduct");

            migrationBuilder.DropTable(
                name: "tblDevice");

            migrationBuilder.DropTable(
                name: "tblContent");

            migrationBuilder.DropTable(
                name: "tblMasterProduct");

            migrationBuilder.DropTable(
                name: "tblProductImport");

            migrationBuilder.DropTable(
                name: "tblPlaylist");

            migrationBuilder.DropTable(
                name: "tblStore");

            migrationBuilder.DropTable(
                name: "tblRetailer");
        }
    }
}
