using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Apptivate_UQMS_WebApp.Migrations
{
    /// <inheritdoc />
    public partial class query_status_type_update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
        name: "Status",
        table: "Queries",
        type: "nvarchar(max)",
        nullable: false,
        oldClrType: typeof(int),
        oldType: "int");


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.AlterColumn<int>(
       name: "Status",
       table: "Queries",
       type: "int",
       nullable: false,
       oldClrType: typeof(string),
       oldType: "nvarchar(max)");
        }
    }
}
