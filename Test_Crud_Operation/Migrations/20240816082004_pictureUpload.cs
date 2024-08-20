using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Test_Crud_Operation.Migrations
{
    public partial class pictureUpload : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "imageUrl",
                table: "Employees",
                type: "varbinary(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "imageUrl",
                table: "Employees");
        }
    }
}
