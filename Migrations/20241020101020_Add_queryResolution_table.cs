using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Apptivate_UQMS_WebApp.Migrations
{
    /// <inheritdoc />
    public partial class Add_queryResolution_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create QueryResolutions table
            migrationBuilder.CreateTable(
       name: "QueryResolutions",
       columns: table => new
       {
           ResolutionID = table.Column<int>(nullable: false)
               .Annotation("SqlServer:Identity", "1, 1"),
           AssignmentID = table.Column<int>(nullable: false),
           QueryID = table.Column<int>(nullable: false),
           Solution = table.Column<string>(nullable: false),
           ApprovalStatus = table.Column<string>(nullable: true),
           AdditionalNotes = table.Column<string>(nullable: true)
       },
       constraints: table =>
       {
           table.PrimaryKey("PK_QueryResolutions", x => x.ResolutionID);
           table.ForeignKey(
               name: "FK_QueryResolutions_QueryAssignments_AssignmentID",
               column: x => x.AssignmentID,
               principalTable: "QueryAssignments",
               principalColumn: "AssignmentID",
               onDelete: ReferentialAction.Cascade);
           table.ForeignKey(
               name: "FK_QueryResolutions_Queries_QueryID",
               column: x => x.QueryID,
               principalTable: "Queries",
               principalColumn: "QueryID",
               onDelete: ReferentialAction.NoAction);  // No cascade here
       });


            // Create ResolutionDocuments table
            migrationBuilder.CreateTable(
                name: "ResolutionDocuments",
                columns: table => new
                {
                    ResolutionID = table.Column<int>(nullable: false),
                    DocumentID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResolutionDocuments", x => new { x.ResolutionID, x.DocumentID });
                    table.ForeignKey(
                        name: "FK_ResolutionDocuments_QueryResolutions_ResolutionID",
                        column: x => x.ResolutionID,
                        principalTable: "QueryResolutions",
                        principalColumn: "ResolutionID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResolutionDocuments_QueryDocuments_DocumentID",
                        column: x => x.DocumentID,
                        principalTable: "QueryDocuments",
                        principalColumn: "DocumentID",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create indexes
            migrationBuilder.CreateIndex(
                name: "IX_QueryResolutions_AssignmentID",
                table: "QueryResolutions",
                column: "AssignmentID");

            migrationBuilder.CreateIndex(
                name: "IX_QueryResolutions_QueryID",
                table: "QueryResolutions",
                column: "QueryID");

            migrationBuilder.CreateIndex(
                name: "IX_ResolutionDocuments_DocumentID",
                table: "ResolutionDocuments",
                column: "DocumentID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ResolutionDocuments");
            migrationBuilder.DropTable(name: "QueryResolutions");
        }

    }
}
