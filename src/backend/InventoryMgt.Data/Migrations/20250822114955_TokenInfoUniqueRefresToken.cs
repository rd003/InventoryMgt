using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryMgt.Data.Migrations
{
    /// <inheritdoc />
    public partial class TokenInfoUniqueRefresToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "uq_token_info_refresh_token",
                table: "token_info",
                column: "refresh_token");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "uq_token_info_refresh_token",
                table: "token_info");
        }
    }
}
