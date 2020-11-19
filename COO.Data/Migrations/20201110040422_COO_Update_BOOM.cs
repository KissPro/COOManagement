using Microsoft.EntityFrameworkCore.Migrations;

namespace COO.Data.Migrations
{
    public partial class COO_Update_BOOM : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Item",
                table: "tbl_Boom",
                unicode: false,
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Level",
                table: "tbl_Boom",
                unicode: false,
                maxLength: 10,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Item",
                table: "tbl_Boom");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "tbl_Boom");
        }
    }
}
