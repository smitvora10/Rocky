using Microsoft.EntityFrameworkCore.Migrations;

namespace Rocky_DataAccess.Migrations
{
    public partial class changedIDNamesInInquiryTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "InquiryHeader",
                newName: "InquiryHeaderId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "InquiryDetail",
                newName: "InquiryDetailId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InquiryHeaderId",
                table: "InquiryHeader",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "InquiryDetailId",
                table: "InquiryDetail",
                newName: "Id");
        }
    }
}
