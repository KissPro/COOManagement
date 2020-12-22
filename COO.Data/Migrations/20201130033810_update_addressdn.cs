using Microsoft.EntityFrameworkCore.Migrations;

namespace COO.Data.Migrations
{
    public partial class update_addressdn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TrackingNo",
                table: "tbl_DSManual",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "COONo",
                table: "tbl_DSManual",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Package",
                table: "tbl_DSManual",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShipFrom",
                table: "tbl_DSManual",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HMDShipToCode",
                table: "tbl_DeliverySales",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShipToCountryName",
                table: "tbl_DeliverySales",
                maxLength: 200,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Package",
                table: "tbl_DSManual");

            migrationBuilder.DropColumn(
                name: "ShipFrom",
                table: "tbl_DSManual");

            migrationBuilder.DropColumn(
                name: "HMDShipToCode",
                table: "tbl_DeliverySales");

            migrationBuilder.DropColumn(
                name: "ShipToCountryName",
                table: "tbl_DeliverySales");

            migrationBuilder.AlterColumn<long>(
                name: "TrackingNo",
                table: "tbl_DSManual",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "COONo",
                table: "tbl_DSManual",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
