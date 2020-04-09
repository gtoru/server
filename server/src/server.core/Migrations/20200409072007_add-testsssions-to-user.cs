using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace server.core.Migrations
{
    public partial class addtestsssionstouser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizTask_Tasks_QuizId",
                table: "QuizTask");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizTask_Quizzes_TaskId",
                table: "QuizTask");

            migrationBuilder.DropForeignKey(
                name: "FK_TestSessions_Users_UserId",
                table: "TestSessions");

            migrationBuilder.DropColumn(
                name: "CurrentSession",
                table: "Users");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "TestSessions",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizTask_Quizzes_QuizId",
                table: "QuizTask",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "QuizId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizTask_Tasks_TaskId",
                table: "QuizTask",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "TaskId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestSessions_Users_UserId",
                table: "TestSessions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizTask_Quizzes_QuizId",
                table: "QuizTask");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizTask_Tasks_TaskId",
                table: "QuizTask");

            migrationBuilder.DropForeignKey(
                name: "FK_TestSessions_Users_UserId",
                table: "TestSessions");

            migrationBuilder.AddColumn<Guid>(
                name: "CurrentSession",
                table: "Users",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "TestSessions",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddForeignKey(
                name: "FK_QuizTask_Tasks_QuizId",
                table: "QuizTask",
                column: "QuizId",
                principalTable: "Tasks",
                principalColumn: "TaskId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizTask_Quizzes_TaskId",
                table: "QuizTask",
                column: "TaskId",
                principalTable: "Quizzes",
                principalColumn: "QuizId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestSessions_Users_UserId",
                table: "TestSessions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
