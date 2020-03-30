using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Routine.Api.Migrations
{
    public partial class adddataemployee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "Introduction", "Name" },
                values: new object[] { new Guid("48a7a966-4c78-4ce5-a300-435507b6cadc"), "Don not be evil", "Google" });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "Introduction", "Name" },
                values: new object[] { new Guid("7dcaf47b-bc1a-4b78-ae5c-ebf952ecf1eb"), "Create Company", "Microsoft" });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "Introduction", "Name" },
                values: new object[] { new Guid("34dc1695-7e1a-4687-824b-5af45aa92342"), "no", "Apple" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CompanyId", "DateOfBirth", "EmployeeNo", "FirstName", "Gender", "LastName" },
                values: new object[] { new Guid("d8a53929-be70-4f51-8fb8-442c3b2c2d6a"), new Guid("48a7a966-4c78-4ce5-a300-435507b6cadc"), new DateTime(2001, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "SF112", "Nike", 1, "Garter" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CompanyId", "DateOfBirth", "EmployeeNo", "FirstName", "Gender", "LastName" },
                values: new object[] { new Guid("656c985a-bcda-4d86-917c-e3775d6ed94c"), new Guid("7dcaf47b-bc1a-4b78-ae5c-ebf952ecf1eb"), new DateTime(2002, 3, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "01487", "Jonty", 1, "Wang" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CompanyId", "DateOfBirth", "EmployeeNo", "FirstName", "Gender", "LastName" },
                values: new object[] { new Guid("a640c260-3ad6-4aba-b86e-817353101fd5"), new Guid("7dcaf47b-bc1a-4b78-ae5c-ebf952ecf1eb"), new DateTime(1987, 4, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "12341", "alice", 2, "Wang" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CompanyId", "DateOfBirth", "EmployeeNo", "FirstName", "Gender", "LastName" },
                values: new object[] { new Guid("ffc7b7ca-52f1-4bb6-a935-f93d238b9733"), new Guid("34dc1695-7e1a-4687-824b-5af45aa92342"), new DateTime(2001, 4, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "11929", "mark", 1, "Wang" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("656c985a-bcda-4d86-917c-e3775d6ed94c"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("a640c260-3ad6-4aba-b86e-817353101fd5"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("d8a53929-be70-4f51-8fb8-442c3b2c2d6a"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("ffc7b7ca-52f1-4bb6-a935-f93d238b9733"));

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: new Guid("34dc1695-7e1a-4687-824b-5af45aa92342"));

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: new Guid("48a7a966-4c78-4ce5-a300-435507b6cadc"));

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: new Guid("7dcaf47b-bc1a-4b78-ae5c-ebf952ecf1eb"));
        }
    }
}
