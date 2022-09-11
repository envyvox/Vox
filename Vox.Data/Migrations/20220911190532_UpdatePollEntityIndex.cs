using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vox.Data.Migrations
{
    public partial class UpdatePollEntityIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_polls_id",
                table: "polls",
                column: "id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_polls_id",
                table: "polls");
        }
    }
}
