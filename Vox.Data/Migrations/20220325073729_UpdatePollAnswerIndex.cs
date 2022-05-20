using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vox.Data.Migrations
{
    public partial class UpdatePollAnswerIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_poll_answers_user_id_poll_id",
                table: "poll_answers");

            migrationBuilder.AlterColumn<string>(
                name: "answer",
                table: "poll_answers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "ix_poll_answers_user_id_poll_id_answer",
                table: "poll_answers",
                columns: new[] { "user_id", "poll_id", "answer" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_poll_answers_user_id_poll_id_answer",
                table: "poll_answers");

            migrationBuilder.AlterColumn<string>(
                name: "answer",
                table: "poll_answers",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_poll_answers_user_id_poll_id",
                table: "poll_answers",
                columns: new[] { "user_id", "poll_id" },
                unique: true);
        }
    }
}
