using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Feed_Bridge.Migrations
{
    /// <inheritdoc />
    public partial class Addrelationbetweendonationanddelivery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Donations_AspNetUsers_UserId",
                table: "Donations");

            migrationBuilder.RenameColumn(
                name: "updatedAt",
                table: "Donations",
                newName: "UpdatedAt");

            migrationBuilder.AddColumn<string>(
                name: "DeliveryId",
                table: "Donations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Donations_DeliveryId",
                table: "Donations",
                column: "DeliveryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Donations_AspNetUsers_DeliveryId",
                table: "Donations",
                column: "DeliveryId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Donations_AspNetUsers_UserId",
                table: "Donations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Donations_AspNetUsers_DeliveryId",
                table: "Donations");

            migrationBuilder.DropForeignKey(
                name: "FK_Donations_AspNetUsers_UserId",
                table: "Donations");

            migrationBuilder.DropIndex(
                name: "IX_Donations_DeliveryId",
                table: "Donations");

            migrationBuilder.DropColumn(
                name: "DeliveryId",
                table: "Donations");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Donations",
                newName: "updatedAt");

            migrationBuilder.AddForeignKey(
                name: "FK_Donations_AspNetUsers_UserId",
                table: "Donations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
