using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutonetProjectMVCASP.Migrations
{
    public partial class StartTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StaryTime",
                table: "Locations",
                newName: "StartTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "Locations",
                newName: "StaryTime");
        }
    }
}
