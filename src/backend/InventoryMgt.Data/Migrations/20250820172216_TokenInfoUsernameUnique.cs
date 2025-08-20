using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryMgt.Data.Migrations
{
    /// <inheritdoc />
    public partial class TokenInfoUsernameUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "uq_token_info_username",
                table: "token_info",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "uq_token_info_username",
                table: "token_info");
        }
    }
}
