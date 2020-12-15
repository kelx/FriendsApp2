using Microsoft.EntityFrameworkCore.Migrations;

namespace FriendsApp2.Api.Migrations
{
    public partial class addedBlockedUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BlockedUser",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockedUser",
                table: "AspNetUsers");
        }
    }
}
