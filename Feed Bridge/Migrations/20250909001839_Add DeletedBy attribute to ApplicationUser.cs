using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Feed_Bridge.Migrations
{
    /// <inheritdoc />
    public partial class AddDeletedByattributetoApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "AspNetUsers");
        }
    }
}
