using Microsoft.EntityFrameworkCore.Migrations;

namespace server.core.Migrations
{
    public partial class addquiznametoquizzes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "QuizName",
                "Quizzes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "QuizName",
                "Quizzes");
        }
    }
}
