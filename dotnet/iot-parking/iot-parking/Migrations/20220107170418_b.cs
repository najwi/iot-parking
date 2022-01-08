using Microsoft.EntityFrameworkCore.Migrations;

namespace iot_parking.Migrations
{
    public partial class b : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardOwners");

            migrationBuilder.DropTable(
                name: "Parkings");

            migrationBuilder.DropTable(
                name: "RFIDCards");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
