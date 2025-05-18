using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Calgon.Host.Migrations
{
    /// <inheritdoc />
    public partial class ModifyRoomsAddCreatedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Rooms",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Rooms");
        }
    }
}
