using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Apptivate_UQMS_WebApp.Migrations
{
    /// <inheritdoc />
    public partial class DropCourseModuleIDColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the CourseModuleID column
            migrationBuilder.DropColumn(
                name: "CourseModuleID",
                table: "CourseModules");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // If you want to reverse this migration, you should add the column back
            migrationBuilder.AddColumn<int>(
                name: "CourseModuleID",
                table: "CourseModules",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

    }
}
