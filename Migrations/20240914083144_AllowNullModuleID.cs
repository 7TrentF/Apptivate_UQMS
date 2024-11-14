using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Apptivate_UQMS_WebApp.Migrations
{
    /// <inheritdoc />
    public partial class AllowNullModuleID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ModuleID",
                table: "Queries",
                type: "INT",
                nullable: true, // Allow nulls
                oldClrType: typeof(int),
                oldType: "INT");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ModuleID",
                table: "Queries",
                type: "INT",
                nullable: false, // Revert to non-nullable
                oldClrType: typeof(int),
                oldType: "INT",
                oldNullable: true);
        }
    }
}
