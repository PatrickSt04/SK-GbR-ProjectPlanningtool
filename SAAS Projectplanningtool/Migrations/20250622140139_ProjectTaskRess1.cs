using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAAS_Projectplanningtool.Migrations
{
    /// <inheritdoc />
    public partial class ProjectTaskRess1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProjectTaskRessourceId",
                table: "ProjectTask",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTask_ProjectTaskRessourceId",
                table: "ProjectTask",
                column: "ProjectTaskRessourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTask_ProjectTaskRessource_ProjectTaskRessourceId",
                table: "ProjectTask",
                column: "ProjectTaskRessourceId",
                principalTable: "ProjectTaskRessource",
                principalColumn: "ProjectTaskRessourceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTask_ProjectTaskRessource_ProjectTaskRessourceId",
                table: "ProjectTask");

            migrationBuilder.DropIndex(
                name: "IX_ProjectTask_ProjectTaskRessourceId",
                table: "ProjectTask");

            migrationBuilder.DropColumn(
                name: "ProjectTaskRessourceId",
                table: "ProjectTask");
        }
    }
}
