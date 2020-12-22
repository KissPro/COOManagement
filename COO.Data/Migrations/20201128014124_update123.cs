using Microsoft.EntityFrameworkCore.Migrations;

namespace COO.Data.Migrations
{
    public partial class update123 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DSRuntime",
                table: "tbl_Config");

            migrationBuilder.DropColumn(
                name: "DSTimeLastMonth",
                table: "tbl_Config");

            migrationBuilder.DropColumn(
                name: "DSTimeLastYear",
                table: "tbl_Config");

            migrationBuilder.DropColumn(
                name: "DSTimeNextMonth",
                table: "tbl_Config");

            migrationBuilder.DropColumn(
                name: "DSTimeNextYear",
                table: "tbl_Config");

            migrationBuilder.DropColumn(
                name: "EcusRuntime",
                table: "tbl_Config");

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "tbl_Config",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "tbl_Config",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Key",
                table: "tbl_Config");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "tbl_Config");

            migrationBuilder.AddColumn<string>(
                name: "DSRuntime",
                table: "tbl_Config",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DSTimeLastMonth",
                table: "tbl_Config",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DSTimeLastYear",
                table: "tbl_Config",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DSTimeNextMonth",
                table: "tbl_Config",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DSTimeNextYear",
                table: "tbl_Config",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EcusRuntime",
                table: "tbl_Config",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
