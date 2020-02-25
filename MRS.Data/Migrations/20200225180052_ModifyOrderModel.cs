using Microsoft.EntityFrameworkCore.Migrations;

namespace MRS.Data.Migrations
{
    public partial class ModifyOrderModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Fullname",
                table: "Order",
                newName: "FullName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Order",
                newName: "Fullname");
        }
    }
}
