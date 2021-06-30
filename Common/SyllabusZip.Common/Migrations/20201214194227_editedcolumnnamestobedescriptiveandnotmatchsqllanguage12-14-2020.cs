using Microsoft.EntityFrameworkCore.Migrations;

namespace SyllabusZip.Migrations
{
    public partial class editedcolumnnamestobedescriptiveandnotmatchsqllanguage12142020 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3fa6c013-54d4-45b6-abf9-ec61588f346f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7b735795-9c47-47eb-b079-a7d724b4af57");

            migrationBuilder.DropColumn(
                name: "When",
                table: "Contact");

            migrationBuilder.DropColumn(
                name: "Where",
                table: "Contact");

            migrationBuilder.DropColumn(
                name: "Who",
                table: "Contact");

            migrationBuilder.AddColumn<string>(
                name: "ClassTime",
                table: "Contact",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Classroom",
                table: "Contact",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Teacher",
                table: "Contact",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "eb01a6da-2241-4317-a330-49e41e04fa33", "3614b653-0689-4dc2-84ce-885ad12c6ddc", "Visitor", "VISITOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "2978309e-fc54-4709-835d-57b9ff6e66d6", "ef1f3c43-ce4f-45a8-9993-630392ec468f", "Administrator", "ADMINISTRATOR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2978309e-fc54-4709-835d-57b9ff6e66d6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "eb01a6da-2241-4317-a330-49e41e04fa33");

            migrationBuilder.DropColumn(
                name: "ClassTime",
                table: "Contact");

            migrationBuilder.DropColumn(
                name: "Classroom",
                table: "Contact");

            migrationBuilder.DropColumn(
                name: "Teacher",
                table: "Contact");

            migrationBuilder.AddColumn<string>(
                name: "When",
                table: "Contact",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Where",
                table: "Contact",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Who",
                table: "Contact",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "3fa6c013-54d4-45b6-abf9-ec61588f346f", "1a1cc342-278f-4a59-87d5-c25ef44acf6a", "Visitor", "VISITOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "7b735795-9c47-47eb-b079-a7d724b4af57", "3e8b13bd-6bfc-47df-b8ae-8e5e9dae35a1", "Administrator", "ADMINISTRATOR" });
        }
    }
}
