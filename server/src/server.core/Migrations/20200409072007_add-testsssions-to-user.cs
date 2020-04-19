using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace server.core.Migrations
{
    public partial class addtestsssionstouser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_QuizTask_Tasks_QuizId",
                "QuizTask");

            migrationBuilder.DropForeignKey(
                "FK_QuizTask_Quizzes_TaskId",
                "QuizTask");

            migrationBuilder.DropForeignKey(
                "FK_TestSessions_Users_UserId",
                "TestSessions");

            migrationBuilder.DropColumn(
                "CurrentSession",
                "Users");

            migrationBuilder.AlterColumn<Guid>(
                "UserId",
                "TestSessions",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                "FK_QuizTask_Quizzes_QuizId",
                "QuizTask",
                "QuizId",
                "Quizzes",
                principalColumn: "QuizId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                "FK_QuizTask_Tasks_TaskId",
                "QuizTask",
                "TaskId",
                "Tasks",
                principalColumn: "TaskId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                "FK_TestSessions_Users_UserId",
                "TestSessions",
                "UserId",
                "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_QuizTask_Quizzes_QuizId",
                "QuizTask");

            migrationBuilder.DropForeignKey(
                "FK_QuizTask_Tasks_TaskId",
                "QuizTask");

            migrationBuilder.DropForeignKey(
                "FK_TestSessions_Users_UserId",
                "TestSessions");

            migrationBuilder.AddColumn<Guid>(
                "CurrentSession",
                "Users",
                "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                "UserId",
                "TestSessions",
                "uuid",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddForeignKey(
                "FK_QuizTask_Tasks_QuizId",
                "QuizTask",
                "QuizId",
                "Tasks",
                principalColumn: "TaskId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                "FK_QuizTask_Quizzes_TaskId",
                "QuizTask",
                "TaskId",
                "Quizzes",
                principalColumn: "QuizId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                "FK_TestSessions_Users_UserId",
                "TestSessions",
                "UserId",
                "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
