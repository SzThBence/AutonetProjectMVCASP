using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutonetProjectMVCASP.Migrations
{
    public partial class employees_for_jobs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Appointments");
        }
    }
}
