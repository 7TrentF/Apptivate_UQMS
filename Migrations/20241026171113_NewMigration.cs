using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Apptivate_UQMS_WebApp.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Conversations",
                columns: table => new
                {
                    ConversationID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User1ID = table.Column<string>(nullable: true),
                    User2ID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.ConversationID);
                });

            migrationBuilder.AddColumn<int>(
                name: "ConversationID",
                table: "Messages",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ConversationID",
                table: "Messages",
                column: "ConversationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Conversations_ConversationID",
                table: "Messages",
                column: "ConversationID",
                principalTable: "Conversations",
                principalColumn: "ConversationID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Conversations_ConversationID",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ConversationID",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ConversationID",
                table: "Messages");

            migrationBuilder.DropTable(
                name: "Conversations");
        }
    }
}