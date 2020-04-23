using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace server.core.Migrations
{
    public partial class addquizrepositories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                "CurrentSession",
                "Users",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                "Quizzes",
                table => new
                {
                    QuizId = table.Column<Guid>(nullable: false),
                    Locked = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table => { table.PrimaryKey("PK_Quizzes", x => x.QuizId); });

            migrationBuilder.CreateTable(
                "Tasks",
                table => new
                {
                    TaskId = table.Column<Guid>(nullable: false),
                    Question = table.Column<string>(nullable: true),
                    Answer = table.Column<string>(nullable: true),
                    Variants = table.Column<List<string>>("jsonb", nullable: true),
                    Locked = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table => { table.PrimaryKey("PK_Tasks", x => x.TaskId); });

            migrationBuilder.CreateTable(
                "TestSessions",
                table => new
                {
                    SessionId = table.Column<Guid>(nullable: false),
                    TimeProvider = table.Column<string>(nullable: true),
                    Started = table.Column<DateTime>(nullable: false),
                    IsFinished = table.Column<bool>(nullable: false, defaultValue: false),
                    Result = table.Column<int>(nullable: false, defaultValue: 0),
                    PossibleResult = table.Column<int>(nullable: false),
                    QuizId = table.Column<Guid>(nullable: true),
                    UserId = table.Column<Guid>(nullable: true),
                    Answers = table.Column<List<string>>("jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestSessions", x => x.SessionId);
                    table.ForeignKey(
                        "FK_TestSessions_Quizzes_QuizId",
                        x => x.QuizId,
                        "Quizzes",
                        "QuizId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_TestSessions_Users_UserId",
                        x => x.UserId,
                        "Users",
                        "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "QuizTask",
                table => new
                {
                    QuizId = table.Column<Guid>(nullable: false),
                    TaskId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizTask", x => new {x.QuizId, x.TaskId});
                    table.ForeignKey(
                        "FK_QuizTask_Tasks_QuizId",
                        x => x.QuizId,
                        "Tasks",
                        "TaskId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_QuizTask_Quizzes_TaskId",
                        x => x.TaskId,
                        "Quizzes",
                        "QuizId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_QuizTask_TaskId",
                "QuizTask",
                "TaskId");

            migrationBuilder.CreateIndex(
                "IX_TestSessions_QuizId",
                "TestSessions",
                "QuizId");

            migrationBuilder.CreateIndex(
                "IX_TestSessions_UserId",
                "TestSessions",
                "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "QuizTask");

            migrationBuilder.DropTable(
                "TestSessions");

            migrationBuilder.DropTable(
                "Tasks");

            migrationBuilder.DropTable(
                "Quizzes");

            migrationBuilder.DropColumn(
                "CurrentSession",
                "Users");
        }
    }
}
