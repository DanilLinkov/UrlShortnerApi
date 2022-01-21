using Microsoft.EntityFrameworkCore.Migrations;

namespace UrlShortner.Migrations
{
    public partial class ChangingShortUrlModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShortenedUrl",
                table: "ShortUrls",
                newName: "ShortenedUrlId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShortenedUrlId",
                table: "ShortUrls",
                newName: "ShortenedUrl");
        }
    }
}
