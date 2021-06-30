using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SyllabusZip.Migrations
{
    public partial class newsyllabusidfieldsinalltables12282020 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0c76af53-131e-4504-abbe-bf3f87d85682");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "880d899c-dfb6-4e12-8d12-af98375eaf85");

            migrationBuilder.AddColumn<Guid>(
                name: "SyllabusId",
                table: "Materials",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SyllabusId",
                table: "Exams",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SyllabusId",
                table: "Contact",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SyllabusId",
                table: "Assignments",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "8248cf16-be04-4e25-af22-3d64b68f8f61", "ebc0230b-1d21-4058-951c-168d770bcaca", "Visitor", "VISITOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "04556ebe-f04b-42f3-8958-49add336fac4", "c6f56350-b606-4493-b7de-b32d6756fa67", "Administrator", "ADMINISTRATOR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "04556ebe-f04b-42f3-8958-49add336fac4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8248cf16-be04-4e25-af22-3d64b68f8f61");

            migrationBuilder.DropColumn(
                name: "SyllabusId",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "SyllabusId",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "SyllabusId",
                table: "Contact");

            migrationBuilder.DropColumn(
                name: "SyllabusId",
                table: "Assignments");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "0c76af53-131e-4504-abbe-bf3f87d85682", "0fa56ffa-89dc-4476-8350-09e809c90db1", "Visitor", "VISITOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "880d899c-dfb6-4e12-8d12-af98375eaf85", "de342b22-dcc4-48d2-a512-c64d5e1f5708", "Administrator", "ADMINISTRATOR" });
        }
    }
}
