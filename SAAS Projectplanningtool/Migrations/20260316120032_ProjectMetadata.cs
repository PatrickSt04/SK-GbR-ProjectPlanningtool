using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAAS_Projectplanningtool.Migrations
{
    /// <inheritdoc />
    public partial class ProjectMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Project_Employee_ResponsiblePersonId",
                table: "Project");

            migrationBuilder.RenameColumn(
                name: "ResponsiblePersonId",
                table: "Project",
                newName: "ProjectLeadId");

            migrationBuilder.RenameIndex(
                name: "IX_Project_ResponsiblePersonId",
                table: "Project",
                newName: "IX_Project_ProjectLeadId");

            migrationBuilder.AddColumn<string>(
                name: "InstructorId",
                table: "Project",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Project_InstructorId",
                table: "Project",
                column: "InstructorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_Employee_InstructorId",
                table: "Project",
                column: "InstructorId",
                principalTable: "Employee",
                principalColumn: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_Employee_ProjectLeadId",
                table: "Project",
                column: "ProjectLeadId",
                principalTable: "Employee",
                principalColumn: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Project_Employee_InstructorId",
                table: "Project");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_Employee_ProjectLeadId",
                table: "Project");

            migrationBuilder.DropIndex(
                name: "IX_Project_InstructorId",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "InstructorId",
                table: "Project");

            migrationBuilder.RenameColumn(
                name: "ProjectLeadId",
                table: "Project",
                newName: "ResponsiblePersonId");

            migrationBuilder.RenameIndex(
                name: "IX_Project_ProjectLeadId",
                table: "Project",
                newName: "IX_Project_ResponsiblePersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_Employee_ResponsiblePersonId",
                table: "Project",
                column: "ResponsiblePersonId",
                principalTable: "Employee",
                principalColumn: "EmployeeId");
        }
    }
}
