using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutonetProjectMVCASP.Migrations
{
    public partial class employees_for_jobs2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocationEmployees",
                columns: table => new
                {
                    LocationPlace = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationEmployees", x => new { x.LocationPlace, x.EmployeeId });
                    table.ForeignKey(
                        name: "FK_LocationEmployees_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LocationEmployees_Locations_LocationPlace",
                        column: x => x.LocationPlace,
                        principalTable: "Locations",
                        principalColumn: "Place",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocationEmployees_EmployeeId",
                table: "LocationEmployees",
                column: "EmployeeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocationEmployees");
        }
    }
}
