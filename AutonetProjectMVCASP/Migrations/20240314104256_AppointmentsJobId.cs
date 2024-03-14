using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutonetProjectMVCASP.Migrations
{
    /// <inheritdoc />
    public partial class AppointmentsJobId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JobId",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobId",
                table: "Appointments");
        }
    }
}
