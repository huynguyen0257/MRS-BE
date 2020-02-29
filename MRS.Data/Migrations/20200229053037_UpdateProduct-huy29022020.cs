using Microsoft.EntityFrameworkCore.Migrations;

namespace MRS.Data.Migrations
{
    public partial class UpdateProducthuy29022020 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Images",
                table: "Product",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MainImage",
                table: "Product",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Images",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "MainImage",
                table: "Product");
        }
    }
}
