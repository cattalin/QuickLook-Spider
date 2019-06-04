using Microsoft.EntityFrameworkCore.Migrations;

namespace QuickLook.WebApi.Migrations
{
    public partial class AddedBookmarkUserRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Bookmark",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookmark_UserId",
                table: "Bookmark",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookmark_User_UserId",
                table: "Bookmark",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookmark_User_UserId",
                table: "Bookmark");

            migrationBuilder.DropIndex(
                name: "IX_Bookmark_UserId",
                table: "Bookmark");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Bookmark");
        }
    }
}
