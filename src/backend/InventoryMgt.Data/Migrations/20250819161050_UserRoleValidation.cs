using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryMgt.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserRoleValidation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Role",
                table: "user",
                newName: "role");

            migrationBuilder.AlterColumn<string>(
                name: "role",
                table: "user",
                type: "character varying(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "role",
                table: "user",
                newName: "Role");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "user",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(15)",
                oldMaxLength: 15);
        }
    }
}
