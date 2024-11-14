using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Apptivate_UQMS_WebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddQueryCategoryAndType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Queries");

            migrationBuilder.AddColumn<int>(
                name: "CategoryID",
                table: "Queries",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "QueryType",
                columns: table => new
                {
                    QueryTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueryType", x => x.QueryTypeID);
                });

            migrationBuilder.CreateTable(
                name: "QueryCategory",
                columns: table => new
                {
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QueryTypeID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueryCategory", x => x.CategoryID);
                    table.ForeignKey(
                        name: "FK_QueryCategory_QueryType_QueryTypeID",
                        column: x => x.QueryTypeID,
                        principalTable: "QueryType",
                        principalColumn: "QueryTypeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Queries_CategoryID",
                table: "Queries",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_QueryCategory_QueryTypeID",
                table: "QueryCategory",
                column: "QueryTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Queries_QueryCategory_CategoryID",
                table: "Queries",
                column: "CategoryID",
                principalTable: "QueryCategory",
                principalColumn: "CategoryID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Queries_QueryCategory_CategoryID",
                table: "Queries");

            migrationBuilder.DropTable(
                name: "QueryCategory");

            migrationBuilder.DropTable(
                name: "QueryType");

            migrationBuilder.DropIndex(
                name: "IX_Queries_CategoryID",
                table: "Queries");

            migrationBuilder.DropColumn(
                name: "CategoryID",
                table: "Queries");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Queries",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
