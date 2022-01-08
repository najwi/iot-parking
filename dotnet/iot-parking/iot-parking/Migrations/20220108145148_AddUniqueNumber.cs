using Microsoft.EntityFrameworkCore.Migrations;

namespace iot_parking.Migrations
{
    public partial class AddUniqueNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CardNumber",
                table: "RFIDCards",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ScannedCards_CardNumber",
                table: "ScannedCards",
                column: "CardNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RFIDCards_CardNumber",
                table: "RFIDCards",
                column: "CardNumber",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ScannedCards_CardNumber",
                table: "ScannedCards");

            migrationBuilder.DropIndex(
                name: "IX_RFIDCards_CardNumber",
                table: "RFIDCards");

            migrationBuilder.AlterColumn<string>(
                name: "CardNumber",
                table: "RFIDCards",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
