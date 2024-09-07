using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Apptivate_UQMS_WebApp.Migrations
{
    /// <inheritdoc />
    public partial class dropBridgingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                  name: "CourseModules");

            migrationBuilder.DropTable(
                name: "DepartmentCourses");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                   name: "CourseModules");

            migrationBuilder.DropTable(
                name: "DepartmentCourses");

        }
    }
}
