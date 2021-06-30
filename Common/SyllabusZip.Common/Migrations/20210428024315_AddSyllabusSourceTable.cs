using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SyllabusZip.Migrations
{
    public partial class AddSyllabusSourceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1bca5018-4385-4a62-a766-5cc3b41af631");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f23ad0aa-322c-4eee-9434-64027141017c");

            migrationBuilder.AddColumn<Guid>(
                name: "SourceId",
                table: "Syllabi",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SyllabusSources",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Service = table.Column<string>(nullable: true),
                    BaseUrl = table.Column<string>(nullable: true),
                    AuthToken = table.Column<string>(nullable: true),
                    AuthTokenExpires = table.Column<DateTime>(nullable: false),
                    RefreshToken = table.Column<string>(nullable: true),
                    RefreshTokenExpires = table.Column<DateTime>(nullable: false),
                    SourceUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyllabusSources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SyllabusSources_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "87c047ed-e5ea-47d3-9fdb-2bbadc7dab0f", "be512b45-a133-4262-ae6f-bfb86ec5e72c", "Visitor", "VISITOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "029300fb-b9f3-4bf1-9abc-ebb19e2c6f33", "b94ef9ff-997f-4074-82b1-41c03bd09916", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.CreateIndex(
                name: "IX_Syllabi_SourceId",
                table: "Syllabi",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_SyllabusSources_UserId",
                table: "SyllabusSources",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Syllabi_SyllabusSources_SourceId",
                table: "Syllabi",
                column: "SourceId",
                principalTable: "SyllabusSources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Syllabi_SyllabusSources_SourceId",
                table: "Syllabi");

            migrationBuilder.DropTable(
                name: "SyllabusSources");

            migrationBuilder.DropIndex(
                name: "IX_Syllabi_SourceId",
                table: "Syllabi");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "029300fb-b9f3-4bf1-9abc-ebb19e2c6f33");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "87c047ed-e5ea-47d3-9fdb-2bbadc7dab0f");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "Syllabi");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "f23ad0aa-322c-4eee-9434-64027141017c", "6e525ae0-d10e-4060-b40d-f9b8e63bc30e", "Visitor", "VISITOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "1bca5018-4385-4a62-a766-5cc3b41af631", "c670402e-5851-4cea-99ef-ef356ac7ed26", "Administrator", "ADMINISTRATOR" });
        }
    }
}
