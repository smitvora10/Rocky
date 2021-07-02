using Microsoft.EntityFrameworkCore.Migrations;

namespace Rocky_DataAccess.Migrations
{
    public partial class renamingColumnsInOrderHeaderAndDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "OrderHeader",
                newName: "OrderHeaderId");

            migrationBuilder.RenameColumn(
                name: "Sqft",
                table: "OrderDetail",
                newName: "SqFt");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "OrderDetail",
                newName: "OrderDetailId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderHeaderId",
                table: "OrderHeader",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "SqFt",
                table: "OrderDetail",
                newName: "Sqft");

            migrationBuilder.RenameColumn(
                name: "OrderDetailId",
                table: "OrderDetail",
                newName: "Id");
        }
    }
}
