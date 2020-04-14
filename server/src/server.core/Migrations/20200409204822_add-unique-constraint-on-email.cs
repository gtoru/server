using Microsoft.EntityFrameworkCore.Migrations;

namespace server.core.Migrations
{
    public partial class adduniqueconstraintonemail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email_Address",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email_Address",
                table: "Users",
                column: "Email_Address",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email_Address",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email_Address",
                table: "Users",
                column: "Email_Address");
        }
    }
}
