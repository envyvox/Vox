using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vox.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "create_channel",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    guild_id = table.Column<long>(type: "bigint", nullable: false),
                    category_id = table.Column<long>(type: "bigint", nullable: false),
                    channel_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_create_channel", x => x.id);
                });

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
                name: "ix_create_channel_guild_id",
                table: "create_channel",
                column: "guild_id");

            migrationBuilder.CreateIndex(
                name: "ix_create_channel_guild_id_category_id",
                table: "create_channel",
                columns: new[] { "guild_id", "category_id" },
                unique: true);

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
                name: "create_channel");

            migrationBuilder.DropTable(
                name: "user_channel");
        }
    }
}
