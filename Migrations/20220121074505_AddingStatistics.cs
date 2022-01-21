using Microsoft.EntityFrameworkCore.Migrations;

namespace UrlShortner.Migrations
{
    public partial class AddingStatistics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Uses",
                table: "ShortUrls",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Uses",
                table: "ShortUrls");
        }
    }
}
