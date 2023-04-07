using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vox.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserChannelEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_poll_answers_polls_poll_id1",
                table: "poll_answers");

            migrationBuilder.DropIndex(
                name: "ix_poll_answers_poll_id1",
                table: "poll_answers");

            migrationBuilder.DropColumn(
                name: "poll_id1",
                table: "poll_answers");

            migrationBuilder.CreateTable(
                name: "user_channel",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    channel_name = table.Column<string>(type: "text", nullable: false),
                    channel_limit = table.Column<int>(type: "integer", nullable: false),
                    permissions = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_channel", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_user_channel_user_id",
                table: "user_channel",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_channel");

            migrationBuilder.AddColumn<Guid>(
                name: "poll_id1",
                table: "poll_answers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_poll_answers_poll_id1",
                table: "poll_answers",
                column: "poll_id1");

            migrationBuilder.AddForeignKey(
                name: "fk_poll_answers_polls_poll_id1",
                table: "poll_answers",
                column: "poll_id1",
                principalTable: "polls",
                principalColumn: "id");
        }
    }
}
