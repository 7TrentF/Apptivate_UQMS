using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Apptivate_UQMS_WebApp.Migrations
{
    /// <inheritdoc />
    public partial class courseModulesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

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

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
              name: "CourseModules");

        }
    }
}
