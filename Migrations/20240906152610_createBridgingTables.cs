using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Apptivate_UQMS_WebApp.Migrations
{
    /// <inheritdoc />
    public partial class createBridgingTables : Migration
    {
        /// <inheritdoc />
        
       protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
               name: "DepartmentCourses",
               columns: table => new
               {
                   DepartmentID = table.Column<int>(type: "int", nullable: false),
                   CourseID = table.Column<int>(type: "int", nullable: false),
                  
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_DepartmentCourses", x => new { x.DepartmentID, x.CourseID });
                   table.ForeignKey(
                       name: "FK_DepartmentCourses_Courses_CourseID",
                       column: x => x.CourseID,
                       principalTable: "Courses",
                       principalColumn: "CourseID",
                       onDelete: ReferentialAction.Cascade);
                   table.ForeignKey(
                       name: "FK_DepartmentCourses_Departments_DepartmentID",
                       column: x => x.DepartmentID,
                       principalTable: "Departments",
                       principalColumn: "DepartmentID",
                       onDelete: ReferentialAction.Cascade);
               });



           

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentCourses_CourseID",
                table: "DepartmentCourses",
                column: "CourseID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable(
                name: "DepartmentCourses");
        }
    }
}
