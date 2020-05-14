using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Routine.Api.Migrations
{
    public partial class addclo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Companies",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Industry",
                table: "Companies",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Product",
                table: "Companies",
                maxLength: 100,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: new Guid("34dc1695-7e1a-4687-824b-5af45aa92342"),
                columns: new[] { "Country", "Industry", "Product" },
                values: new object[] { "USA", "Internet", "Software" });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: new Guid("48a7a966-4c78-4ce5-a300-435507b6cadc"),
                columns: new[] { "Country", "Industry", "Product" },
                values: new object[] { "USA", "Software", "Software" });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: new Guid("7dcaf47b-bc1a-4b78-ae5c-ebf952ecf1eb"),
                columns: new[] { "Country", "Industry", "Product" },
                values: new object[] { "USA", "Internet", "Software" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Industry",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Product",
                table: "Companies");
        }
    }
}
