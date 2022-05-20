using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vox.Data.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "guilds",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_guilds", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "guild_create_channels",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    guild_id = table.Column<long>(type: "bigint", nullable: false),
                    category_id = table.Column<long>(type: "bigint", nullable: false),
                    channel_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_guild_create_channels", x => x.id);
                    table.ForeignKey(
                        name: "fk_guild_create_channels_guilds_guild_id",
                        column: x => x.guild_id,
                        principalTable: "guilds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_guild_create_channels_guild_id",
                table: "guild_create_channels",
                column: "guild_id");

            migrationBuilder.CreateIndex(
                name: "ix_guild_create_channels_guild_id_category_id",
                table: "guild_create_channels",
                columns: new[] { "guild_id", "category_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_guilds_id",
                table: "guilds",
                column: "id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "guild_create_channels");

            migrationBuilder.DropTable(
                name: "guilds");
        }
    }
}
