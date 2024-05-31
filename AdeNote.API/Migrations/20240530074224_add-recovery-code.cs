using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdeNote.Migrations
{
    /// <inheritdoc />
    public partial class addrecoverycode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecoveryCode_Users_UserId",
                table: "RecoveryCode");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RecoveryCode",
                table: "RecoveryCode");

            migrationBuilder.RenameTable(
                name: "RecoveryCode",
                newName: "RecoveryCodes");

            migrationBuilder.RenameIndex(
                name: "IX_RecoveryCode_UserId",
                table: "RecoveryCodes",
                newName: "IX_RecoveryCodes_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RecoveryCodes",
                table: "RecoveryCodes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RecoveryCodes_Users_UserId",
                table: "RecoveryCodes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecoveryCodes_Users_UserId",
                table: "RecoveryCodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RecoveryCodes",
                table: "RecoveryCodes");

            migrationBuilder.RenameTable(
                name: "RecoveryCodes",
                newName: "RecoveryCode");

            migrationBuilder.RenameIndex(
                name: "IX_RecoveryCodes_UserId",
                table: "RecoveryCode",
                newName: "IX_RecoveryCode_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RecoveryCode",
                table: "RecoveryCode",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RecoveryCode_Users_UserId",
                table: "RecoveryCode",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
