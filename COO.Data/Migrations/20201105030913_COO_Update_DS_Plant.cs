using Microsoft.EntityFrameworkCore.Migrations;

namespace COO.Data.Migrations
{
    public partial class COO_Update_DS_Plant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemarkCountry",
                table: "tbl_Plant");

            migrationBuilder.DropColumn(
                name: "CountryName",
                table: "tbl_CountryShip");

            migrationBuilder.DropColumn(
                name: "ShipID",
                table: "tbl_CountryShip");

            migrationBuilder.AddColumn<string>(
                name: "RemarkPlant",
                table: "tbl_Plant",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HMDShipToCode",
                table: "tbl_CountryShip",
                fixedLength: true,
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HMDShipToParty",
                table: "tbl_CountryShip",
                unicode: false,
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShipToCountryCode",
                table: "tbl_CountryShip",
                fixedLength: true,
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShipToCountryName",
                table: "tbl_CountryShip",
                unicode: false,
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemarkPlant",
                table: "tbl_Plant");

            migrationBuilder.DropColumn(
                name: "HMDShipToCode",
                table: "tbl_CountryShip");

            migrationBuilder.DropColumn(
                name: "HMDShipToParty",
                table: "tbl_CountryShip");

            migrationBuilder.DropColumn(
                name: "ShipToCountryCode",
                table: "tbl_CountryShip");

            migrationBuilder.DropColumn(
                name: "ShipToCountryName",
                table: "tbl_CountryShip");

            migrationBuilder.AddColumn<string>(
                name: "RemarkCountry",
                table: "tbl_Plant",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CountryName",
                table: "tbl_CountryShip",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShipID",
                table: "tbl_CountryShip",
                type: "nchar(10)",
                fixedLength: true,
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }
    }
}
