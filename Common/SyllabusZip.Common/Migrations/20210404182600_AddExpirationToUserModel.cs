using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SyllabusZip.Migrations
{
    public partial class AddExpirationToUserModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "30aec4d8-a318-49ff-ae75-4faece60519a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3a6859bf-c9ca-4f99-8b37-1e41b4b511d1");

            migrationBuilder.AddColumn<DateTime>(
                name: "Expiration",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "0adae4ef-8980-4157-9b25-fafc86babf51", "f6dda5c1-f6b0-4fd8-a257-d80eda182728", "Visitor", "VISITOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e89f42a1-cd3a-4dbb-bd0c-44998ad897fd", "f0874825-ce6c-4ecd-a276-c2ce81f49875", "Administrator", "ADMINISTRATOR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0adae4ef-8980-4157-9b25-fafc86babf51");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e89f42a1-cd3a-4dbb-bd0c-44998ad897fd");

            migrationBuilder.DropColumn(
                name: "Expiration",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "3a6859bf-c9ca-4f99-8b37-1e41b4b511d1", "bbb53654-138b-4d33-aecb-99c4f07b1311", "Visitor", "VISITOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "30aec4d8-a318-49ff-ae75-4faece60519a", "fb832292-3e81-40cb-878e-b5eade5ea72b", "Administrator", "ADMINISTRATOR" });
        }
    }
}
