using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Apptivate_UQMS_WebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddQueryCategoryAndQueryType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Queries_QueryCategory_CategoryID",
                table: "Queries");

            migrationBuilder.DropForeignKey(
                name: "FK_QueryCategory_QueryType_QueryTypeID",
                table: "QueryCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QueryType",
                table: "QueryType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QueryCategory",
                table: "QueryCategory");

            migrationBuilder.RenameTable(
                name: "QueryType",
                newName: "QueryTypes");

            migrationBuilder.RenameTable(
                name: "QueryCategory",
                newName: "QueryCategories");

            migrationBuilder.RenameIndex(
                name: "IX_QueryCategory_QueryTypeID",
                table: "QueryCategories",
                newName: "IX_QueryCategories_QueryTypeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QueryTypes",
                table: "QueryTypes",
                column: "QueryTypeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QueryCategories",
                table: "QueryCategories",
                column: "CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_Queries_QueryCategories_CategoryID",
                table: "Queries",
                column: "CategoryID",
                principalTable: "QueryCategories",
                principalColumn: "CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_QueryCategories_QueryTypes_QueryTypeID",
                table: "QueryCategories",
                column: "QueryTypeID",
                principalTable: "QueryTypes",
                principalColumn: "QueryTypeID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Queries_QueryCategories_CategoryID",
                table: "Queries");

            migrationBuilder.DropForeignKey(
                name: "FK_QueryCategories_QueryTypes_QueryTypeID",
                table: "QueryCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QueryTypes",
                table: "QueryTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QueryCategories",
                table: "QueryCategories");

            migrationBuilder.RenameTable(
                name: "QueryTypes",
                newName: "QueryType");

            migrationBuilder.RenameTable(
                name: "QueryCategories",
                newName: "QueryCategory");

            migrationBuilder.RenameIndex(
                name: "IX_QueryCategories_QueryTypeID",
                table: "QueryCategory",
                newName: "IX_QueryCategory_QueryTypeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QueryType",
                table: "QueryType",
                column: "QueryTypeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QueryCategory",
                table: "QueryCategory",
                column: "CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_Queries_QueryCategory_CategoryID",
                table: "Queries",
                column: "CategoryID",
                principalTable: "QueryCategory",
                principalColumn: "CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_QueryCategory_QueryType_QueryTypeID",
                table: "QueryCategory",
                column: "QueryTypeID",
                principalTable: "QueryType",
                principalColumn: "QueryTypeID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
