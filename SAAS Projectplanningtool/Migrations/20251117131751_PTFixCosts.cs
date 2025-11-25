using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAAS_Projectplanningtool.Migrations
{
    /// <inheritdoc />
    public partial class PTFixCosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectTaskFixCosts",
                columns: table => new
                {
                    ProjectTaskFixCostsId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProjectTaskId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FixCosts = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTaskFixCosts", x => x.ProjectTaskFixCostsId);
                    table.ForeignKey(
                        name: "FK_ProjectTaskFixCosts_ProjectTask_ProjectTaskId",
                        column: x => x.ProjectTaskId,
                        principalTable: "ProjectTask",
                        principalColumn: "ProjectTaskId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskFixCosts_ProjectTaskId",
                table: "ProjectTaskFixCosts",
                column: "ProjectTaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectTaskFixCosts");
        }
    }
}
