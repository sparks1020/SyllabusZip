using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SyllabusZip.Migrations
{
    public partial class AddCourseFirstDay : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "029300fb-b9f3-4bf1-9abc-ebb19e2c6f33");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "87c047ed-e5ea-47d3-9fdb-2bbadc7dab0f");

            migrationBuilder.AddColumn<DateTime>(
                name: "CourseFirstDay",
                table: "Syllabi",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "73351ec7-a96e-42c0-9ec9-fd3449ff46de", "fe48663e-1356-43c8-aeef-9cb47d0aa9af", "Visitor", "VISITOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "320bafa2-b4ad-4e33-ba6c-cf095acc1e83", "23572bbd-fcd0-4b5a-a191-1433b93e7488", "Administrator", "ADMINISTRATOR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "320bafa2-b4ad-4e33-ba6c-cf095acc1e83");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "73351ec7-a96e-42c0-9ec9-fd3449ff46de");

            migrationBuilder.DropColumn(
                name: "CourseFirstDay",
                table: "Syllabi");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "87c047ed-e5ea-47d3-9fdb-2bbadc7dab0f", "be512b45-a133-4262-ae6f-bfb86ec5e72c", "Visitor", "VISITOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "029300fb-b9f3-4bf1-9abc-ebb19e2c6f33", "b94ef9ff-997f-4074-82b1-41c03bd09916", "Administrator", "ADMINISTRATOR" });
        }
    }
}
