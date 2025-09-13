using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Feed_Bridge.Migrations
{
    /// <inheritdoc />
    public partial class AddIsFrozenattribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFrozen",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFrozen",
                table: "AspNetUsers");
        }
    }
}
