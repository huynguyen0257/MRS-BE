using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MRS.Data.Migrations
{
    public partial class ModifyModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Order",
                newName: "Fullname");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrderId",
                table: "Carts",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "Carts",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "Carts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Account_Id",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Device_Id",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "Account_Id",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Device_Id",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "Fullname",
                table: "Order",
                newName: "UserName");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrderId",
                table: "Carts",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);
        }
    }
}
