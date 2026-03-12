using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsureYouAI.Migrations
{
    /// <inheritdoc />
    public partial class mg3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Surname",
                table: "Messages",
                newName: "Subject");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Messages",
                newName: "NameSurname");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SendDate",
                table: "Messages",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsRead",
                table: "Messages",
                type: "bit",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Subject",
                table: "Messages",
                newName: "Surname");

            migrationBuilder.RenameColumn(
                name: "NameSurname",
                table: "Messages",
                newName: "Name");

            migrationBuilder.AlterColumn<string>(
                name: "SendDate",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "IsRead",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
