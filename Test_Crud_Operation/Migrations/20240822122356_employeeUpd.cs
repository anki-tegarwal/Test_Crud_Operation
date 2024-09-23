using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Test_Crud_Operation.Migrations
{
    public partial class employeeUpd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "imageUrl",
                table: "Employees");

            migrationBuilder.CreateTable(
                name: "EmployeeImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeImages_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeImages_EmployeeId",
                table: "EmployeeImages",
                column: "EmployeeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeImages");

            migrationBuilder.AddColumn<byte[]>(
                name: "imageUrl",
                table: "Employees",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
