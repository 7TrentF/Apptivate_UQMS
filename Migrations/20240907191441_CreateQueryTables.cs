using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Apptivate_UQMS_WebApp.Migrations
{
    /// <inheritdoc />
    public partial class CreateQueryTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Queries",
                columns: table => new
                {
                    QueryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartmentID = table.Column<int>(type: "int", nullable: false),
                    CourseID = table.Column<int>(type: "int", nullable: false),
                    ModuleID = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResolvedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Queries", x => x.QueryID);
                    table.ForeignKey(
                        name: "FK_Queries_Courses_CourseID",
                        column: x => x.CourseID,
                        principalTable: "Courses",
                        principalColumn: "CourseID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Queries_Departments_DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "Departments",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Queries_Modules_ModuleID",
                        column: x => x.ModuleID,
                        principalTable: "Modules",
                        principalColumn: "ModuleID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Queries_StudentDetails_StudentID",
                        column: x => x.StudentID,
                        principalTable: "StudentDetails",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
     name: "Feedback",
     columns: table => new
     {
         FeedbackID = table.Column<int>(type: "int", nullable: false)
             .Annotation("SqlServer:Identity", "1, 1"),
         QueryID = table.Column<int>(type: "int", nullable: false),
         StudentID = table.Column<int>(type: "int", nullable: false),
         Rating = table.Column<int>(type: "int", nullable: false),
         Comments = table.Column<string>(type: "nvarchar(max)", nullable: false),
         SubmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
     },
     constraints: table =>
     {
         table.PrimaryKey("PK_Feedback", x => x.FeedbackID);
         table.ForeignKey(
             name: "FK_Feedback_Queries_QueryID",
             column: x => x.QueryID,
             principalTable: "Queries",
             principalColumn: "QueryID",
             onDelete: ReferentialAction.Cascade); // Keep cascade delete
         table.ForeignKey(
             name: "FK_Feedback_StudentDetails_StudentID",
             column: x => x.StudentID,
             principalTable: "StudentDetails",
             principalColumn: "StudentID",
             onDelete: ReferentialAction.Restrict); // Remove cascade delete here
     });



            migrationBuilder.CreateTable(
                name: "QueryAssignments",
                columns: table => new
                {
                    AssignmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QueryID = table.Column<int>(type: "int", nullable: false),
                    StaffID = table.Column<int>(type: "int", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResolutionDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueryAssignments", x => x.AssignmentID);
                    table.ForeignKey(
                        name: "FK_QueryAssignments_Queries_QueryID",
                        column: x => x.QueryID,
                        principalTable: "Queries",
                        principalColumn: "QueryID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QueryAssignments_StaffDetails_StaffID",
                        column: x => x.StaffID,
                        principalTable: "StaffDetails",
                        principalColumn: "StaffID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QueryDocuments",
                columns: table => new
                {
                    DocumentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QueryID = table.Column<int>(type: "int", nullable: false),
                    DocumentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueryDocuments", x => x.DocumentID);
                    table.ForeignKey(
                        name: "FK_QueryDocuments_Queries_QueryID",
                        column: x => x.QueryID,
                        principalTable: "Queries",
                        principalColumn: "QueryID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_QueryID",
                table: "Feedback",
                column: "QueryID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_StudentID",
                table: "Feedback",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_Queries_CourseID",
                table: "Queries",
                column: "CourseID");

            migrationBuilder.CreateIndex(
                name: "IX_Queries_DepartmentID",
                table: "Queries",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Queries_ModuleID",
                table: "Queries",
                column: "ModuleID");

            migrationBuilder.CreateIndex(
                name: "IX_Queries_StudentID",
                table: "Queries",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_QueryAssignments_QueryID",
                table: "QueryAssignments",
                column: "QueryID");

            migrationBuilder.CreateIndex(
                name: "IX_QueryAssignments_StaffID",
                table: "QueryAssignments",
                column: "StaffID");

            migrationBuilder.CreateIndex(
                name: "IX_QueryDocuments_QueryID",
                table: "QueryDocuments",
                column: "QueryID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Feedback");

            migrationBuilder.DropTable(
                name: "QueryAssignments");

            migrationBuilder.DropTable(
                name: "QueryDocuments");

            migrationBuilder.DropTable(
                name: "Queries");
        }
    }
}
