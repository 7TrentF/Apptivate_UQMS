using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Apptivate_UQMS_WebApp.Migrations
{
    /// <inheritdoc />
    public partial class NewChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Chats_ChatId",
                table: "Messages");

            migrationBuilder.DropTable(
                name: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ChatId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "RecipientEmail",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "SenderEmail",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "MessageId",
                table: "Messages",
                newName: "MessageID");

            migrationBuilder.RenameColumn(
                name: "SentAt",
                table: "Messages",
                newName: "Timestamp");

            migrationBuilder.RenameColumn(
                name: "SenderUserId",
                table: "Messages",
                newName: "SenderID");

            migrationBuilder.RenameColumn(
                name: "ChatId",
                table: "Messages",
                newName: "ReceiverID");

            migrationBuilder.AddColumn<string>(
                name: "ConnectionId",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConnectionId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "MessageID",
                table: "Messages",
                newName: "MessageId");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "Messages",
                newName: "SentAt");

            migrationBuilder.RenameColumn(
                name: "SenderID",
                table: "Messages",
                newName: "SenderUserId");

            migrationBuilder.RenameColumn(
                name: "ReceiverID",
                table: "Messages",
                newName: "ChatId");

            migrationBuilder.AddColumn<string>(
                name: "RecipientEmail",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SenderEmail",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    ChatId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecipientUserId = table.Column<int>(type: "int", nullable: false),
                    SenderUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.ChatId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatId",
                table: "Messages",
                column: "ChatId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Chats_ChatId",
                table: "Messages",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "ChatId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
