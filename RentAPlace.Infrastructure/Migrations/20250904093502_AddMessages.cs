using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentAPlace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Properties_PropertyId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_FromUserId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_ToUserId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_PropertyId",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "ToUserId",
                table: "Messages",
                newName: "SenderId");

            migrationBuilder.RenameColumn(
                name: "SentAt",
                table: "Messages",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "FromUserId",
                table: "Messages",
                newName: "RecipientId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_ToUserId",
                table: "Messages",
                newName: "IX_Messages_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_FromUserId",
                table: "Messages",
                newName: "IX_Messages_RecipientId");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Messages",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_PropertyId_SenderId_RecipientId_CreatedAt",
                table: "Messages",
                columns: new[] { "PropertyId", "SenderId", "RecipientId", "CreatedAt" });

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Properties_PropertyId",
                table: "Messages",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "PropertyId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_RecipientId",
                table: "Messages",
                column: "RecipientId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_SenderId",
                table: "Messages",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Properties_PropertyId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_RecipientId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_SenderId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_PropertyId_SenderId_RecipientId_CreatedAt",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "SenderId",
                table: "Messages",
                newName: "ToUserId");

            migrationBuilder.RenameColumn(
                name: "RecipientId",
                table: "Messages",
                newName: "FromUserId");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Messages",
                newName: "SentAt");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                newName: "IX_Messages_ToUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_RecipientId",
                table: "Messages",
                newName: "IX_Messages_FromUserId");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_PropertyId",
                table: "Messages",
                column: "PropertyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Properties_PropertyId",
                table: "Messages",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "PropertyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_FromUserId",
                table: "Messages",
                column: "FromUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_ToUserId",
                table: "Messages",
                column: "ToUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
