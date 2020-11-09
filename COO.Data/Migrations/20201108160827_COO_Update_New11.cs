using Microsoft.EntityFrameworkCore.Migrations;

namespace COO.Data.Migrations
{
    public partial class COO_Update_New11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_tblBoom_1",
                table: "tbl_Boom");

            migrationBuilder.AlterColumn<string>(
                name: "Material",
                table: "tbl_Boom",
                unicode: false,
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50);

            migrationBuilder.AddPrimaryKey(
                name: "PK_tblBoom_1",
                table: "tbl_Boom",
                column: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_tblBoom_1",
                table: "tbl_Boom");

            migrationBuilder.AlterColumn<string>(
                name: "Material",
                table: "tbl_Boom",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_tblBoom_1",
                table: "tbl_Boom",
                column: "Material");
        }
    }
}
