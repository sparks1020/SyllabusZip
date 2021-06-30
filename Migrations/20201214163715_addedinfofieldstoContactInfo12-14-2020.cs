using Microsoft.EntityFrameworkCore.Migrations;

namespace SyllabusZip.Migrations
{
    public partial class addedinfofieldstoContactInfo12142020 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "64462da9-f139-4683-89c8-eea27721ae4d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bf0a4315-03f8-452f-bca6-8019b28fff15");

            migrationBuilder.AddColumn<string>(
                name: "ClassTitle",
                table: "Contact",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "When",
                table: "Contact",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Where",
                table: "Contact",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Who",
                table: "Contact",
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

        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "ClassTitle",
                table: "Contact");

            migrationBuilder.DropColumn(
                name: "When",
                table: "Contact");

            migrationBuilder.DropColumn(
                name: "Where",
                table: "Contact");

            migrationBuilder.DropColumn(
                name: "Who",
                table: "Contact");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "64462da9-f139-4683-89c8-eea27721ae4d", "10dbc3f8-ef4c-4a46-9a72-491baadd687c", "Visitor", "VISITOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "bf0a4315-03f8-452f-bca6-8019b28fff15", "5ab8807d-8099-420c-9fce-2a75a7327309", "Administrator", "ADMINISTRATOR" });
        }
    }
}
