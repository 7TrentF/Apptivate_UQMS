using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Apptivate_UQMS_WebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddPositionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Position",
                table: "StaffDetails");

            migrationBuilder.AddColumn<int>(
                name: "PositionID",
                table: "StaffDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    PositionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PositionName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.PositionID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StaffDetails_PositionID",
                table: "StaffDetails",
                column: "PositionID");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffDetails_Positions_PositionID",
                table: "StaffDetails",
                column: "PositionID",
                principalTable: "Positions",
                principalColumn: "PositionID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaffDetails_Positions_PositionID",
                table: "StaffDetails");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropIndex(
                name: "IX_StaffDetails_PositionID",
                table: "StaffDetails");

            migrationBuilder.DropColumn(
                name: "PositionID",
                table: "StaffDetails");

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "StaffDetails",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
