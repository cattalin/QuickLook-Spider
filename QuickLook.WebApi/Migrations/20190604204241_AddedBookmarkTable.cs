using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace QuickLook.WebApi.Migrations
{
    public partial class AddedBookmarkTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bookmark",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ElasticsearchId = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    Language = table.Column<string>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookmark", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookmark");
        }
    }
}
