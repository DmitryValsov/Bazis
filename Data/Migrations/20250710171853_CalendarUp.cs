using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bazis.Data.Migrations
{
    /// <inheritdoc />
    public partial class CalendarUp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "ServiceCenters",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MapLink",
                table: "ServiceCenters",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpeningHours",
                table: "ServiceCenters",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "ServiceCenters",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Rating",
                table: "ServiceCenters",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "ServiceCenters");

            migrationBuilder.DropColumn(
                name: "MapLink",
                table: "ServiceCenters");

            migrationBuilder.DropColumn(
                name: "OpeningHours",
                table: "ServiceCenters");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "ServiceCenters");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "ServiceCenters");
        }
    }
}
