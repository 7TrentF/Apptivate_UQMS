using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Apptivate_UQMS_WebApp.Migrations
{
    /// <inheritdoc />
    public partial class NewChatting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    MessageID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderEmail = table.Column<string>(maxLength: 254, nullable: false),
                    ReceiverEmail = table.Column<string>(maxLength: 254, nullable: false),
                    Content = table.Column<string>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    IsRead = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.MessageID);
                });

            // Indexes for faster search by sender or receiver
            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderEmail",
                table: "Messages",
                column: "SenderEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReceiverEmail",
                table: "Messages",
                column: "ReceiverEmail");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");
        }

    }
}
