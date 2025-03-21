using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAAS_Projectplanningtool.Migrations
{
    /// <inheritdoc />
    public partial class ProjectResponsibleP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResponsiblePersonId",
                table: "Project",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Project_ResponsiblePersonId",
                table: "Project",
                column: "ResponsiblePersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_Employee_ResponsiblePersonId",
                table: "Project",
                column: "ResponsiblePersonId",
                principalTable: "Employee",
                principalColumn: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Project_Employee_ResponsiblePersonId",
                table: "Project");

            migrationBuilder.DropIndex(
                name: "IX_Project_ResponsiblePersonId",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "ResponsiblePersonId",
                table: "Project");
        }
    }
}
