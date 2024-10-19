#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Apptivate_UQMS_WebApp.Migrations
{
    /// <inheritdoc />
    public partial class NewConvo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create the Messages table
            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    MessageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiverID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: DateTime.UtcNow),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ConversationID = table.Column<int>(type: "int", nullable: false) // Add ConversationID as a foreign key
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.MessageID);
                });

            // Create the Conversations table
            migrationBuilder.CreateTable(
                name: "Conversations",
                columns: table => new
                {
                    ConversationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User1ID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    User2ID = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.ConversationID);
                });

            // Create indexes and foreign keys
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
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Conversations_ConversationID",
                table: "Messages");

            migrationBuilder.DropTable(
                name: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ConversationID",
                table: "Messages");

            migrationBuilder.DropTable(
                name: "Messages");
        }
    }
}
