using Microsoft.EntityFrameworkCore.Migrations;

namespace MRS.Data.Migrations
{
    public partial class updateProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfLike",
                table: "Product",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfLike",
                table: "Product");
        }
    }
}
