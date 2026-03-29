using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsureYouAI.Migrations
{
    /// <inheritdoc />
    public partial class mig27 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Policy_AspNetUsers_AppUserId1",
                table: "Policy");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Policy",
                table: "Policy");

            migrationBuilder.DropIndex(
                name: "IX_Policy_AppUserId1",
                table: "Policy");

            migrationBuilder.DropColumn(
                name: "AppUserId1",
                table: "Policy");

            migrationBuilder.RenameTable(
                name: "Policy",
                newName: "Policies");

            migrationBuilder.AlterColumn<string>(
                name: "AppUserId",
                table: "Policies",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Policies",
                table: "Policies",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_AppUserId",
                table: "Policies",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Policies_AspNetUsers_AppUserId",
                table: "Policies",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Policies_AspNetUsers_AppUserId",
                table: "Policies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Policies",
                table: "Policies");

            migrationBuilder.DropIndex(
                name: "IX_Policies_AppUserId",
                table: "Policies");

            migrationBuilder.RenameTable(
                name: "Policies",
                newName: "Policy");

            migrationBuilder.AlterColumn<int>(
                name: "AppUserId",
                table: "Policy",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "AppUserId1",
                table: "Policy",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Policy",
                table: "Policy",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_Policy_AppUserId1",
                table: "Policy",
                column: "AppUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Policy_AspNetUsers_AppUserId1",
                table: "Policy",
                column: "AppUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
