using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace COO.Data.Migrations
{
    public partial class COO_Add_tblBoomEcus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_BoomEcusTS",
                columns: table => new
                {
                    MaHS = table.Column<string>(unicode: false, maxLength: 20, nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    DonGiaHD = table.Column<decimal>(type: "decimal(18, 4)", nullable: true),
                    Country = table.Column<string>(unicode: false, maxLength: 20, nullable: true),
                    SoTK = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    NgayDK = table.Column<DateTime>(type: "datetime", nullable: true),
                    ParentMaterial = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    SortString = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    AltGroup = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    Plant = table.Column<string>(unicode: false, maxLength: 10, nullable: false),
                    TenHang = table.Column<string>(maxLength: 500, nullable: true),
                    Level = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    Item = table.Column<string>(unicode: false, maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_BoomEcusTS");
        }
    }
}
