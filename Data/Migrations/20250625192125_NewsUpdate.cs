using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bazis.Data.Migrations
{
    /// <inheritdoc />
    public partial class NewsUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Img",
                table: "News",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Img",
                table: "News");
        }
    }
}
