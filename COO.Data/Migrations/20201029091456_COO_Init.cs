using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace COO.Data.Migrations
{
    public partial class COO_Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_Boom",
                columns: table => new
                {
                    Material = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    ID = table.Column<Guid>(nullable: false),
                    ParentMaterial = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    SortString = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    AltGroup = table.Column<int>(nullable: true),
                    Plant = table.Column<string>(unicode: false, maxLength: 10, nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblBoom_1", x => x.Material);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Config",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    EcusRuntime = table.Column<string>(maxLength: 50, nullable: false),
                    DSRuntime = table.Column<string>(maxLength: 50, nullable: false),
                    DSTimeLastMonth = table.Column<int>(nullable: false),
                    DSTimeNextMonth = table.Column<int>(nullable: false),
                    DSTimeLastYear = table.Column<int>(nullable: false),
                    DSTimeNextYear = table.Column<int>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 50, nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    RemarkConfig = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Config", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_CountryShip",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    ShipID = table.Column<string>(fixedLength: true, maxLength: 10, nullable: false),
                    CountryName = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 50, nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    RemarkCountry = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_CountryShip", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_DeliverySales",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Delivery = table.Column<long>(nullable: false),
                    InvoiceNo = table.Column<long>(nullable: false),
                    MaterialParent = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    MaterialDesc = table.Column<string>(maxLength: 500, nullable: false),
                    ShipToCountry = table.Column<string>(unicode: false, maxLength: 10, nullable: false),
                    PartyName = table.Column<string>(maxLength: 200, nullable: false),
                    CustomerInvoiceNo = table.Column<long>(nullable: false),
                    SaleUnit = table.Column<string>(unicode: false, maxLength: 10, nullable: false),
                    ActualGIDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    NetValue = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    DNQty = table.Column<long>(nullable: false),
                    NetPrice = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    PlanGIDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    PlanGISysDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_DeliverySales", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_EcusTS",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    SoTK = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    NgayDK = table.Column<DateTime>(type: "datetime", nullable: true),
                    MaHS = table.Column<string>(unicode: false, maxLength: 20, nullable: true),
                    MaHang = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    TenHang = table.Column<string>(maxLength: 500, nullable: true),
                    DonGiaHD = table.Column<decimal>(type: "decimal(18, 4)", nullable: true),
                    Country = table.Column<string>(unicode: false, maxLength: 20, nullable: true),
                    InsertedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_EcusTS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Plant",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Plant = table.Column<string>(fixedLength: true, maxLength: 10, nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 50, nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    RemarkCountry = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Plant", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_DSManual",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    DeliverySalesID = table.Column<Guid>(nullable: false),
                    COONo = table.Column<long>(nullable: true),
                    ReceiptDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ReturnDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    COOForm = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    TrackingNo = table.Column<long>(nullable: true),
                    CourierDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    TrackingDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Origin = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    UpdatedBy = table.Column<string>(maxLength: 50, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    RemarkDS = table.Column<string>(fixedLength: true, maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_DSManual", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tblDSManual_tblDeliverySales",
                        column: x => x.DeliverySalesID,
                        principalTable: "tbl_DeliverySales",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_DSManual_DeliverySalesID",
                table: "tbl_DSManual",
                column: "DeliverySalesID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_Boom");

            migrationBuilder.DropTable(
                name: "tbl_Config");

            migrationBuilder.DropTable(
                name: "tbl_CountryShip");

            migrationBuilder.DropTable(
                name: "tbl_DSManual");

            migrationBuilder.DropTable(
                name: "tbl_EcusTS");

            migrationBuilder.DropTable(
                name: "tbl_Plant");

            migrationBuilder.DropTable(
                name: "tbl_DeliverySales");
        }
    }
}
