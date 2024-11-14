using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Apptivate_UQMS_WebApp.Migrations
{
    /// <inheritdoc />
    public partial class UserAndRoleSpecificTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "NVARCHAR(100)", nullable: false),
                    Surname = table.Column<string>(type: "NVARCHAR(100)", nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR(100)", nullable: false),
                    PasswordHash = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    Role = table.Column<string>(type: "NVARCHAR(50)", nullable: false),
                    RegistrationDate = table.Column<DateTime>(nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID);
                    table.UniqueConstraint("AK_Email", x => x.Email);
                });

            migrationBuilder.CreateTable(
                name: "StudentDetails",
                columns: table => new
                {
                    StudentID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(nullable: false),
                    Department = table.Column<string>(type: "NVARCHAR(100)", nullable: false),
                    Course = table.Column<string>(type: "NVARCHAR(100)", nullable: false),
                    Year = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentDetails", x => x.StudentID);
                    table.ForeignKey(
                        name: "FK_StudentDetails_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StaffDetails",
                columns: table => new
                {
                    StaffID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(nullable: false),
                    Department = table.Column<string>(type: "NVARCHAR(100)", nullable: false),
                    Position = table.Column<string>(type: "NVARCHAR(100)", nullable: false),
                    YearGroupTeaching = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffDetails", x => x.StaffID);
                    table.ForeignKey(
                        name: "FK_StaffDetails_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdminDetails",
                columns: table => new
                {
                    AdminID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(nullable: false),
                    Department = table.Column<string>(type: "NVARCHAR(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminDetails", x => x.AdminID);
                    table.ForeignKey(
                        name: "FK_AdminDetails_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create indexes for foreign keys
            migrationBuilder.CreateIndex(
                name: "IX_StudentDetails_UserID",
                table: "StudentDetails",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_StaffDetails_UserID",
                table: "StaffDetails",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_AdminDetails_UserID",
                table: "AdminDetails",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "StudentDetails");
            migrationBuilder.DropTable(name: "StaffDetails");
            migrationBuilder.DropTable(name: "AdminDetails");
            migrationBuilder.DropTable(name: "Users");
        }

    }
}
