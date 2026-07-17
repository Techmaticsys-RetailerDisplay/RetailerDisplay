using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RetailerDisplay.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRetailerProfileFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddressLine1",
                table: "tblRetailer",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressLine2",
                table: "tblRetailer",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "tblRetailer",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "tblRetailer",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "tblRetailer",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ProfileCompleted",
                table: "tblRetailer",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "tblRetailer",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressLine1",
                table: "tblRetailer");

            migrationBuilder.DropColumn(
                name: "AddressLine2",
                table: "tblRetailer");

            migrationBuilder.DropColumn(
                name: "City",
                table: "tblRetailer");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "tblRetailer");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "tblRetailer");

            migrationBuilder.DropColumn(
                name: "ProfileCompleted",
                table: "tblRetailer");

            migrationBuilder.DropColumn(
                name: "State",
                table: "tblRetailer");
        }
    }
}
