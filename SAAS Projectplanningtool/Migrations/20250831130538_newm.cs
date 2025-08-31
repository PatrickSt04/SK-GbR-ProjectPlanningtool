using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAAS_Projectplanningtool.Migrations
{
    /// <inheritdoc />
    public partial class newm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_ProjectTaskRessource_ProjectTaskRessourceId",
                table: "Employee");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTask_ProjectTaskRessource_ProjectTaskRessourceId",
                table: "ProjectTask");

            migrationBuilder.DropTable(
                name: "ProjectTaskRessource");

            migrationBuilder.DropIndex(
                name: "IX_ProjectTask_ProjectTaskRessourceId",
                table: "ProjectTask");

            migrationBuilder.DropIndex(
                name: "IX_Employee_ProjectTaskRessourceId",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "ProjectTaskRessourceId",
                table: "ProjectTask");

            migrationBuilder.DropColumn(
                name: "ProjectTaskRessourceId",
                table: "Employee");

            migrationBuilder.CreateTable(
                name: "ProjectTaskHourlyRateGroup",
                columns: table => new
                {
                    ProjectTaskHourlyRateGroupId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProjectTaskId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HourlyRateGroupId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTaskHourlyRateGroup", x => x.ProjectTaskHourlyRateGroupId);
                    table.ForeignKey(
                        name: "FK_ProjectTaskHourlyRateGroup_HourlyRateGroup_HourlyRateGroupId",
                        column: x => x.HourlyRateGroupId,
                        principalTable: "HourlyRateGroup",
                        principalColumn: "HourlyRateGroupId");
                    table.ForeignKey(
                        name: "FK_ProjectTaskHourlyRateGroup_ProjectTask_ProjectTaskId",
                        column: x => x.ProjectTaskId,
                        principalTable: "ProjectTask",
                        principalColumn: "ProjectTaskId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskHourlyRateGroup_HourlyRateGroupId",
                table: "ProjectTaskHourlyRateGroup",
                column: "HourlyRateGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskHourlyRateGroup_ProjectTaskId",
                table: "ProjectTaskHourlyRateGroup",
                column: "ProjectTaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectTaskHourlyRateGroup");

            migrationBuilder.AddColumn<string>(
                name: "ProjectTaskRessourceId",
                table: "ProjectTask",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectTaskRessourceId",
                table: "Employee",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProjectTaskRessource",
                columns: table => new
                {
                    ProjectTaskRessourceId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProjectTaskId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestModificationText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LatestModificationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProjectTaskName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTaskRessource", x => x.ProjectTaskRessourceId);
                    table.ForeignKey(
                        name: "FK_ProjectTaskRessource_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_ProjectTaskRessource_Employee_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectTaskRessource_Employee_LatestModifierId",
                        column: x => x.LatestModifierId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectTaskRessource_ProjectTask_ProjectTaskId",
                        column: x => x.ProjectTaskId,
                        principalTable: "ProjectTask",
                        principalColumn: "ProjectTaskId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTask_ProjectTaskRessourceId",
                table: "ProjectTask",
                column: "ProjectTaskRessourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_ProjectTaskRessourceId",
                table: "Employee",
                column: "ProjectTaskRessourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskRessource_CompanyId",
                table: "ProjectTaskRessource",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskRessource_CreatedById",
                table: "ProjectTaskRessource",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskRessource_LatestModifierId",
                table: "ProjectTaskRessource",
                column: "LatestModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskRessource_ProjectTaskId",
                table: "ProjectTaskRessource",
                column: "ProjectTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_ProjectTaskRessource_ProjectTaskRessourceId",
                table: "Employee",
                column: "ProjectTaskRessourceId",
                principalTable: "ProjectTaskRessource",
                principalColumn: "ProjectTaskRessourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTask_ProjectTaskRessource_ProjectTaskRessourceId",
                table: "ProjectTask",
                column: "ProjectTaskRessourceId",
                principalTable: "ProjectTaskRessource",
                principalColumn: "ProjectTaskRessourceId");
        }
    }
}
