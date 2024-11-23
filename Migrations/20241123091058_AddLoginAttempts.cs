using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Apptivate_UQMS_WebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddLoginAttempts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
         name: "LoginAttempts",
         columns: table => new
         {
             Id = table.Column<int>(type: "int", nullable: false)
                 .Annotation("SqlServer:Identity", "1, 1"),
             IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
             Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
             AttemptCount = table.Column<int>(type: "int", nullable: false),
             FirstAttemptAt = table.Column<DateTime>(type: "datetime2", nullable: false),
             LockoutEnd = table.Column<DateTime>(type: "datetime2", nullable: true)
         },
         constraints: table =>
         {
             table.PrimaryKey("PK_LoginAttempts", x => x.Id);
         });

            migrationBuilder.CreateIndex(
                name: "IX_LoginAttempts_IpAddress_Email",
                table: "LoginAttempts",
                columns: new[] { "IpAddress", "Email" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoginAttempts");
        }
    }
}
