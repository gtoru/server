using Microsoft.EntityFrameworkCore.Migrations;

namespace server.core.Migrations
{
    public partial class adduniqueconstraintonemail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                "IX_Users_Email_Address",
                "Users");

            migrationBuilder.CreateIndex(
                "IX_Users_Email_Address",
                "Users",
                "Email_Address",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                "IX_Users_Email_Address",
                "Users");

            migrationBuilder.CreateIndex(
                "IX_Users_Email_Address",
                "Users",
                "Email_Address");
        }
    }
}
