using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAAS_Projectplanningtool.Migrations
{
    /// <inheritdoc />
    public partial class mig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTask_ProjectSection_ProjectSectionID",
                table: "ProjectTask");

            migrationBuilder.RenameColumn(
                name: "ProjectSectionID",
                table: "ProjectTask",
                newName: "ProjectSectionId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectTask_ProjectSectionID",
                table: "ProjectTask",
                newName: "IX_ProjectTask_ProjectSectionId");

            migrationBuilder.AddColumn<string>(
                name: "StateId",
                table: "ProjectTask",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StateId",
                table: "ProjectSection",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StateId",
                table: "Project",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTask_StateId",
                table: "ProjectTask",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSection_StateId",
                table: "ProjectSection",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_StateId",
                table: "Project",
                column: "StateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_State_StateId",
                table: "Project",
                column: "StateId",
                principalTable: "State",
                principalColumn: "StateId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectSection_State_StateId",
                table: "ProjectSection",
                column: "StateId",
                principalTable: "State",
                principalColumn: "StateId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTask_ProjectSection_ProjectSectionId",
                table: "ProjectTask",
                column: "ProjectSectionId",
                principalTable: "ProjectSection",
                principalColumn: "ProjectSectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTask_State_StateId",
                table: "ProjectTask",
                column: "StateId",
                principalTable: "State",
                principalColumn: "StateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Project_State_StateId",
                table: "Project");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectSection_State_StateId",
                table: "ProjectSection");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTask_ProjectSection_ProjectSectionId",
                table: "ProjectTask");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTask_State_StateId",
                table: "ProjectTask");

            migrationBuilder.DropIndex(
                name: "IX_ProjectTask_StateId",
                table: "ProjectTask");

            migrationBuilder.DropIndex(
                name: "IX_ProjectSection_StateId",
                table: "ProjectSection");

            migrationBuilder.DropIndex(
                name: "IX_Project_StateId",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "StateId",
                table: "ProjectTask");

            migrationBuilder.DropColumn(
                name: "StateId",
                table: "ProjectSection");

            migrationBuilder.DropColumn(
                name: "StateId",
                table: "Project");

            migrationBuilder.RenameColumn(
                name: "ProjectSectionId",
                table: "ProjectTask",
                newName: "ProjectSectionID");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectTask_ProjectSectionId",
                table: "ProjectTask",
                newName: "IX_ProjectTask_ProjectSectionID");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTask_ProjectSection_ProjectSectionID",
                table: "ProjectTask",
                column: "ProjectSectionID",
                principalTable: "ProjectSection",
                principalColumn: "ProjectSectionId");
        }
    }
}
