using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace COO.Data.Migrations
{
    public partial class Update_DN_temp1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_DeliverySales_Temp",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Delivery = table.Column<long>(nullable: false),
                    InvoiceNo = table.Column<long>(nullable: false),
                    MaterialParent = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    MaterialDesc = table.Column<string>(maxLength: 500, nullable: false),
                    ShipToCountry = table.Column<string>(unicode: false, maxLength: 10, nullable: false),
                    ShipToCountryName = table.Column<string>(maxLength: 200, nullable: true),
                    HMDShipToCode = table.Column<string>(maxLength: 200, nullable: true),
                    PartyName = table.Column<string>(maxLength: 200, nullable: true),
                    CustomerInvoiceNo = table.Column<long>(nullable: true),
                    SaleUnit = table.Column<string>(unicode: false, maxLength: 10, nullable: false),
                    ActualGIDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    NetValue = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    DNQty = table.Column<long>(nullable: false),
                    NetPrice = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    HarmonizationCode = table.Column<string>(maxLength: 200, nullable: true),
                    Address = table.Column<string>(maxLength: 500, nullable: true),
                    Plant = table.Column<string>(maxLength: 10, nullable: true),
                    PlanGIDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    PlanGISysDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_DeliverySales_Temp", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_DeliverySales_Temp");
        }
    }
}
