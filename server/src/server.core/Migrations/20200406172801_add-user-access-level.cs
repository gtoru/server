using Microsoft.EntityFrameworkCore.Migrations;

namespace server.core.Migrations
{
    public partial class adduseraccesslevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccessLevel",
                table: "Users",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessLevel",
                table: "Users");
        }
    }
}
