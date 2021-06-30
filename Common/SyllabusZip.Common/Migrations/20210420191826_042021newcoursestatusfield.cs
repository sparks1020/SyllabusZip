using Microsoft.EntityFrameworkCore.Migrations;

namespace SyllabusZip.Migrations
{
    public partial class _042021newcoursestatusfield : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0adae4ef-8980-4157-9b25-fafc86babf51");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e89f42a1-cd3a-4dbb-bd0c-44998ad897fd");

            migrationBuilder.AddColumn<bool>(
                name: "CourseStatus",
                table: "Syllabi",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "f23ad0aa-322c-4eee-9434-64027141017c", "6e525ae0-d10e-4060-b40d-f9b8e63bc30e", "Visitor", "VISITOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "1bca5018-4385-4a62-a766-5cc3b41af631", "c670402e-5851-4cea-99ef-ef356ac7ed26", "Administrator", "ADMINISTRATOR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1bca5018-4385-4a62-a766-5cc3b41af631");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f23ad0aa-322c-4eee-9434-64027141017c");

            migrationBuilder.DropColumn(
                name: "CourseStatus",
                table: "Syllabi");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "0adae4ef-8980-4157-9b25-fafc86babf51", "f6dda5c1-f6b0-4fd8-a257-d80eda182728", "Visitor", "VISITOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e89f42a1-cd3a-4dbb-bd0c-44998ad897fd", "f0874825-ce6c-4ecd-a276-c2ce81f49875", "Administrator", "ADMINISTRATOR" });
        }
    }
}
