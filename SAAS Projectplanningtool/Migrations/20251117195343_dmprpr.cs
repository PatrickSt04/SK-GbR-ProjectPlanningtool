using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAAS_Projectplanningtool.Migrations
{
    /// <inheritdoc />
    public partial class dmprpr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ProjectTaskFixCostsId",
                table: "ProjectTask",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTask_ProjectTaskFixCostsId",
                table: "ProjectTask",
                column: "ProjectTaskFixCostsId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTask_ProjectTaskFixCosts_ProjectTaskFixCostsId",
                table: "ProjectTask",
                column: "ProjectTaskFixCostsId",
                principalTable: "ProjectTaskFixCosts",
                principalColumn: "ProjectTaskFixCostsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTask_ProjectTaskFixCosts_ProjectTaskFixCostsId",
                table: "ProjectTask");

            migrationBuilder.DropIndex(
                name: "IX_ProjectTask_ProjectTaskFixCostsId",
                table: "ProjectTask");

            migrationBuilder.AlterColumn<string>(
                name: "ProjectTaskFixCostsId",
                table: "ProjectTask",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
