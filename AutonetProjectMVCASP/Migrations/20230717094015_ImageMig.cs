using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutonetProjectMVCASP.Migrations
{
    public partial class ImageMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Locations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Employees");
        }
    }
}
