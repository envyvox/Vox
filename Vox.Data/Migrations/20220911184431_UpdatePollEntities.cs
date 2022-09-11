using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vox.Data.Migrations
{
    public partial class UpdatePollEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_polls_guilds_guild_id",
                table: "polls");

            migrationBuilder.DropIndex(
                name: "ix_polls_guild_id_channel_id_message_id",
                table: "polls");

            migrationBuilder.DropIndex(
                name: "ix_poll_answers_poll_id",
                table: "poll_answers");

            migrationBuilder.DropIndex(
                name: "ix_poll_answers_user_id_poll_id_answer",
                table: "poll_answers");

            migrationBuilder.DropColumn(
                name: "channel_id",
                table: "polls");

            migrationBuilder.DropColumn(
                name: "guild_id",
                table: "polls");

            migrationBuilder.DropColumn(
                name: "message_id",
                table: "polls");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "poll_answers");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "poll_answers");

            migrationBuilder.AddColumn<Guid>(
                name: "poll_id1",
                table: "poll_answers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "user_poll_answers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    poll_id = table.Column<Guid>(type: "uuid", nullable: false),
                    answer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_poll_answers", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_poll_answers_poll_answers_answer_id",
                        column: x => x.answer_id,
                        principalTable: "poll_answers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_poll_answers_polls_poll_id",
                        column: x => x.poll_id,
                        principalTable: "polls",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_poll_answers_poll_id_answer",
                table: "poll_answers",
                columns: new[] { "poll_id", "answer" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_poll_answers_poll_id1",
                table: "poll_answers",
                column: "poll_id1");

            migrationBuilder.CreateIndex(
                name: "ix_user_poll_answers_answer_id",
                table: "user_poll_answers",
                column: "answer_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_poll_answers_poll_id",
                table: "user_poll_answers",
                column: "poll_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_poll_answers_user_id_poll_id_answer_id",
                table: "user_poll_answers",
                columns: new[] { "user_id", "poll_id", "answer_id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_poll_answers_polls_poll_id1",
                table: "poll_answers",
                column: "poll_id1",
                principalTable: "polls",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_poll_answers_polls_poll_id1",
                table: "poll_answers");

            migrationBuilder.DropTable(
                name: "user_poll_answers");

            migrationBuilder.DropIndex(
                name: "ix_poll_answers_poll_id_answer",
                table: "poll_answers");

            migrationBuilder.DropIndex(
                name: "ix_poll_answers_poll_id1",
                table: "poll_answers");

            migrationBuilder.DropColumn(
                name: "poll_id1",
                table: "poll_answers");

            migrationBuilder.AddColumn<long>(
                name: "channel_id",
                table: "polls",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "guild_id",
                table: "polls",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "message_id",
                table: "polls",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "poll_answers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<long>(
                name: "user_id",
                table: "poll_answers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "ix_polls_guild_id_channel_id_message_id",
                table: "polls",
                columns: new[] { "guild_id", "channel_id", "message_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_poll_answers_poll_id",
                table: "poll_answers",
                column: "poll_id");

            migrationBuilder.CreateIndex(
                name: "ix_poll_answers_user_id_poll_id_answer",
                table: "poll_answers",
                columns: new[] { "user_id", "poll_id", "answer" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_polls_guilds_guild_id",
                table: "polls",
                column: "guild_id",
                principalTable: "guilds",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
