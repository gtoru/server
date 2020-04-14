using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace server.core.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Users",
                table => new
                {
                    UserId = table.Column<Guid>(),
                    Password_HashAlgorithm = table.Column<int>(nullable: true),
                    Password_HashedPassword = table.Column<string>(nullable: true),
                    Email_IsVerified = table.Column<bool>(nullable: true),
                    Email_Address = table.Column<string>(nullable: true),
                    PersonalInfo_Name = table.Column<string>(nullable: true),
                    PersonalInfo_Birthday = table.Column<DateTime>(nullable: true),
                    PersonalInfo_Address = table.Column<string>(nullable: true),
                    PersonalInfo_Occupation = table.Column<string>(nullable: true),
                    PersonalInfo_Employer = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Users", x => x.UserId); });

            migrationBuilder.CreateIndex(
                "IX_Users_Email_Address",
                "Users",
                "Email_Address");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Users");
        }
    }
}
