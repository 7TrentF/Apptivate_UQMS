using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Apptivate_UQMS_WebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentsCoursesModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    CourseID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CourseName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.CourseID);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DepartmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.DepartmentID);
                });

            migrationBuilder.CreateTable(
                name: "Modules",
                columns: table => new
                {
                    ModuleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModuleCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModuleName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.ModuleID);
                });

            migrationBuilder.CreateTable(
                name: "DepartmentCourses",
                columns: table => new
                {
                    DepartmentID = table.Column<int>(type: "int", nullable: false),
                    CourseID = table.Column<int>(type: "int", nullable: false),
                    DepartmentCourseID = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "CourseModules",
                columns: table => new
                {
                    CourseID = table.Column<int>(type: "int", nullable: false),
                    ModuleID = table.Column<int>(type: "int", nullable: false),
                    CourseModuleID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseModules", x => new { x.CourseID, x.ModuleID });
                    table.ForeignKey(
                        name: "FK_CourseModules_Courses_CourseID",
                        column: x => x.CourseID,
                        principalTable: "Courses",
                        principalColumn: "CourseID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseModules_Modules_ModuleID",
                        column: x => x.ModuleID,
                        principalTable: "Modules",
                        principalColumn: "ModuleID",
                        onDelete: ReferentialAction.Cascade);
                });


            migrationBuilder.CreateIndex(
                name: "IX_CourseModules_ModuleID",
                table: "CourseModules",
                column: "ModuleID");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentCourses_CourseID",
                table: "DepartmentCourses",
                column: "CourseID");
        }

     

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.DropTable(
                name: "CourseModules");

            migrationBuilder.DropTable(
                name: "DepartmentCourses");


            migrationBuilder.DropTable(
                name: "Modules");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Departments");

           
        }
    }
}
