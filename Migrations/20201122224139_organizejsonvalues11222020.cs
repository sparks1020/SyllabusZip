using Microsoft.EntityFrameworkCore.Migrations;

namespace SyllabusZip.Migrations
{
    public partial class organizejsonvalues11222020 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2add8898-6786-4d50-9936-86cdb488d3bc");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f8add6c5-accd-4f83-95eb-d10fc0159516");

            migrationBuilder.DropColumn(
                name: "Books",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "Tools",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Materials");

            migrationBuilder.AddColumn<string>(
                name: "Material_Type",
                table: "Materials",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Material_Value",
                table: "Materials",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "64462da9-f139-4683-89c8-eea27721ae4d", "10dbc3f8-ef4c-4a46-9a72-491baadd687c", "Visitor", "VISITOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "bf0a4315-03f8-452f-bca6-8019b28fff15", "5ab8807d-8099-420c-9fce-2a75a7327309", "Administrator", "ADMINISTRATOR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "64462da9-f139-4683-89c8-eea27721ae4d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bf0a4315-03f8-452f-bca6-8019b28fff15");

            migrationBuilder.DropColumn(
                name: "Material_Type",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "Material_Value",
                table: "Materials");

            migrationBuilder.AddColumn<string>(
                name: "Books",
                table: "Materials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tools",
                table: "Materials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Materials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "f8add6c5-accd-4f83-95eb-d10fc0159516", "f9975f92-a9da-4955-acfb-36f32c00630c", "Visitor", "VISITOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "2add8898-6786-4d50-9936-86cdb488d3bc", "2c64c798-53d2-4de8-80be-a96999beb2c1", "Administrator", "ADMINISTRATOR" });
        }
    }
}
