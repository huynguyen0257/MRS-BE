using Microsoft.EntityFrameworkCore.Migrations;

namespace MRS.Data.Migrations
{
    public partial class UpdateMainImageOrDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductMainImage",
                table: "OrderDetails",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductMainImage",
                table: "OrderDetails");
        }
    }
}
