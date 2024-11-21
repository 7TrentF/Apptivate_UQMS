using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Apptivate_UQMS_WebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddKnowledgeBaseAndFaqTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create KnowledgeBase table
            migrationBuilder.CreateTable(
                name: "KnowledgeBase",
                columns: table => new
                {
                    ArticleID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"), // Auto-increment primary key
                    Title = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    Category = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    LastUpdatedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeBase", x => x.ArticleID);
                });

            // Create FAQ table
            migrationBuilder.CreateTable(
                name: "FAQ",
                columns: table => new
                {
                    FaqId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"), // Auto-increment primary key
                    ArticleID = table.Column<int>(nullable: false), // Foreign key column
                    Question = table.Column<string>(nullable: true),
                    Answer = table.Column<string>(nullable: true),
                    Category = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    LastUpdatedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAQ", x => x.FaqId);
                    table.ForeignKey(
                        name: "FK_FAQ_KnowledgeBase_ArticleID",
                        column: x => x.ArticleID,
                        principalTable: "KnowledgeBase",
                        principalColumn: "ArticleID",
                        onDelete: ReferentialAction.Cascade); // Enforce foreign key relationship
                });

            // Create index for FAQ table on ArticleID for faster queries
            migrationBuilder.CreateIndex(
                name: "IX_FAQ_ArticleID",
                table: "FAQ",
                column: "ArticleID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop FAQ table first due to dependency
            migrationBuilder.DropTable(
                name: "FAQ");

            // Drop KnowledgeBase table
            migrationBuilder.DropTable(
                name: "KnowledgeBase");
        }





    }
}
