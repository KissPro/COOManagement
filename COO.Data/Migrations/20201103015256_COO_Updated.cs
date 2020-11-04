using Microsoft.EntityFrameworkCore.Migrations;

namespace COO.Data.Migrations
{
    public partial class COO_Updated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "tbl_DeliverySales",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HarmonizationCode",
                table: "tbl_DeliverySales",
                maxLength: 200,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "tbl_DeliverySales");

            migrationBuilder.DropColumn(
                name: "HarmonizationCode",
                table: "tbl_DeliverySales");
        }
    }
}
