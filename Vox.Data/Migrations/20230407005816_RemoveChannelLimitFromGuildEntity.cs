using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vox.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveChannelLimitFromGuildEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_user_channel",
                table: "user_channel");

            migrationBuilder.DropColumn(
                name: "create_room_limit",
                table: "guilds");

            migrationBuilder.RenameTable(
                name: "user_channel",
                newName: "user_channels");

            migrationBuilder.RenameColumn(
                name: "permissions",
                table: "user_channels",
                newName: "overwrites_data");

            migrationBuilder.RenameIndex(
                name: "ix_user_channel_user_id",
                table: "user_channels",
                newName: "ix_user_channels_user_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_user_channels",
                table: "user_channels",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_user_channels",
                table: "user_channels");

            migrationBuilder.RenameTable(
                name: "user_channels",
                newName: "user_channel");

            migrationBuilder.RenameColumn(
                name: "overwrites_data",
                table: "user_channel",
                newName: "permissions");

            migrationBuilder.RenameIndex(
                name: "ix_user_channels_user_id",
                table: "user_channel",
                newName: "ix_user_channel_user_id");

            migrationBuilder.AddColumn<int>(
                name: "create_room_limit",
                table: "guilds",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "pk_user_channel",
                table: "user_channel",
                column: "id");
        }
    }
}
