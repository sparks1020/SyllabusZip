using Microsoft.EntityFrameworkCore.Migrations;

namespace SyllabusZip.Migrations
{
    public partial class AddCustomerIdToUserModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "04556ebe-f04b-42f3-8958-49add336fac4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8248cf16-be04-4e25-af22-3d64b68f8f61");

            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "3a6859bf-c9ca-4f99-8b37-1e41b4b511d1", "bbb53654-138b-4d33-aecb-99c4f07b1311", "Visitor", "VISITOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "30aec4d8-a318-49ff-ae75-4faece60519a", "fb832292-3e81-40cb-878e-b5eade5ea72b", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.CreateIndex(
                name: "IX_Exams_SyllabusId",
                table: "Exams",
                column: "SyllabusId");

            migrationBuilder.CreateIndex(
                name: "IX_Contact_SyllabusId",
                table: "Contact",
                column: "SyllabusId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_SyllabusId",
                table: "Assignments",
                column: "SyllabusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Syllabi_SyllabusId",
                table: "Assignments",
                column: "SyllabusId",
                principalTable: "Syllabi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contact_Syllabi_SyllabusId",
                table: "Contact",
                column: "SyllabusId",
                principalTable: "Syllabi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_Syllabi_SyllabusId",
                table: "Exams",
                column: "SyllabusId",
                principalTable: "Syllabi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Syllabi_SyllabusId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Contact_Syllabi_SyllabusId",
                table: "Contact");

            migrationBuilder.DropForeignKey(
                name: "FK_Exams_Syllabi_SyllabusId",
                table: "Exams");

            migrationBuilder.DropIndex(
                name: "IX_Exams_SyllabusId",
                table: "Exams");

            migrationBuilder.DropIndex(
                name: "IX_Contact_SyllabusId",
                table: "Contact");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_SyllabusId",
                table: "Assignments");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "30aec4d8-a318-49ff-ae75-4faece60519a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3a6859bf-c9ca-4f99-8b37-1e41b4b511d1");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "8248cf16-be04-4e25-af22-3d64b68f8f61", "ebc0230b-1d21-4058-951c-168d770bcaca", "Visitor", "VISITOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "04556ebe-f04b-42f3-8958-49add336fac4", "c6f56350-b606-4493-b7de-b32d6756fa67", "Administrator", "ADMINISTRATOR" });
        }
    }
}
