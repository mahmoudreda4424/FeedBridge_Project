using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Feed_Bridge.Migrations
{
    /// <inheritdoc />
    public partial class AddCretedAtattributetodonation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Donations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "updatedAt",
                table: "Donations",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Donations");

            migrationBuilder.DropColumn(
                name: "updatedAt",
                table: "Donations");
        }
    }
}
