using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Routine.Api.Migrations
{
    public partial class editdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CompanyId", "DateOfBirth", "EmployeeNo", "FirstName", "Gender", "LastName" },
                values: new object[] { new Guid("cf58b318-4c49-4f7a-bfaa-5227a1e48afd"), new Guid("7dcaf47b-bc1a-4b78-ae5c-ebf952ecf1eb"), new DateTime(2001, 5, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "11324", "alice", 1, "jonty" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CompanyId", "DateOfBirth", "EmployeeNo", "FirstName", "Gender", "LastName" },
                values: new object[] { new Guid("8b320533-67e5-4db6-bf3d-d87a24311d25"), new Guid("7dcaf47b-bc1a-4b78-ae5c-ebf952ecf1eb"), new DateTime(2000, 7, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "34521", "alice", 2, "wang" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("8b320533-67e5-4db6-bf3d-d87a24311d25"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("cf58b318-4c49-4f7a-bfaa-5227a1e48afd"));
        }
    }
}
