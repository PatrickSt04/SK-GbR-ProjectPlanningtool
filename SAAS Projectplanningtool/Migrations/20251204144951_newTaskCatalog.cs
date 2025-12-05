using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAAS_Projectplanningtool.Migrations
{
    /// <inheritdoc />
    public partial class newTaskCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTask_ProjectTaskFixCosts_ProjectTaskFixCostsId",
                table: "ProjectTask");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTaskFixCosts_ProjectTask_ProjectTaskId",
                table: "ProjectTaskFixCosts");

            migrationBuilder.DropIndex(
                name: "IX_ProjectTaskFixCosts_ProjectTaskId",
                table: "ProjectTaskFixCosts");

            migrationBuilder.DropIndex(
                name: "IX_ProjectTask_ProjectTaskFixCostsId",
                table: "ProjectTask");

            migrationBuilder.DropColumn(
                name: "IsScheduleEntry",
                table: "ProjectTask");

            migrationBuilder.DropColumn(
                name: "IsTaskCatalogEntry",
                table: "ProjectTask");

            migrationBuilder.DropColumn(
                name: "ProjectTaskFixCostsId",
                table: "ProjectTask");

            migrationBuilder.RenameColumn(
                name: "ProjectTaskId",
                table: "ProjectTaskFixCosts",
                newName: "TaskId");

            migrationBuilder.CreateTable(
                name: "ProjectTaskCatalogTask",
                columns: table => new
                {
                    ProjectTaskCatalogTaskId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TaskName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProjectId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModificationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestModificationText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StateId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProjectTaskFixCostsId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTaskCatalogTask", x => x.ProjectTaskCatalogTaskId);
                    table.ForeignKey(
                        name: "FK_ProjectTaskCatalogTask_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_ProjectTaskCatalogTask_Employee_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_ProjectTaskCatalogTask_Employee_LatestModifierId",
                        column: x => x.LatestModifierId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_ProjectTaskCatalogTask_ProjectTaskFixCosts_ProjectTaskFixCostsId",
                        column: x => x.ProjectTaskFixCostsId,
                        principalTable: "ProjectTaskFixCosts",
                        principalColumn: "ProjectTaskFixCostsId");
                    table.ForeignKey(
                        name: "FK_ProjectTaskCatalogTask_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId");
                    table.ForeignKey(
                        name: "FK_ProjectTaskCatalogTask_State_StateId",
                        column: x => x.StateId,
                        principalTable: "State",
                        principalColumn: "StateId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskFixCosts_TaskId",
                table: "ProjectTaskFixCosts",
                column: "TaskId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskCatalogTask_CompanyId",
                table: "ProjectTaskCatalogTask",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskCatalogTask_CreatedById",
                table: "ProjectTaskCatalogTask",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskCatalogTask_LatestModifierId",
                table: "ProjectTaskCatalogTask",
                column: "LatestModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskCatalogTask_ProjectId",
                table: "ProjectTaskCatalogTask",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskCatalogTask_ProjectTaskFixCostsId",
                table: "ProjectTaskCatalogTask",
                column: "ProjectTaskFixCostsId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskCatalogTask_StateId",
                table: "ProjectTaskCatalogTask",
                column: "StateId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTaskFixCosts_ProjectTaskCatalogTask_TaskId",
                table: "ProjectTaskFixCosts",
                column: "TaskId",
                principalTable: "ProjectTaskCatalogTask",
                principalColumn: "ProjectTaskCatalogTaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTaskFixCosts_ProjectTaskCatalogTask_TaskId",
                table: "ProjectTaskFixCosts");

            migrationBuilder.DropTable(
                name: "ProjectTaskCatalogTask");

            migrationBuilder.DropIndex(
                name: "IX_ProjectTaskFixCosts_TaskId",
                table: "ProjectTaskFixCosts");

            migrationBuilder.RenameColumn(
                name: "TaskId",
                table: "ProjectTaskFixCosts",
                newName: "ProjectTaskId");

            migrationBuilder.AddColumn<bool>(
                name: "IsScheduleEntry",
                table: "ProjectTask",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTaskCatalogEntry",
                table: "ProjectTask",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ProjectTaskFixCostsId",
                table: "ProjectTask",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskFixCosts_ProjectTaskId",
                table: "ProjectTaskFixCosts",
                column: "ProjectTaskId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTaskFixCosts_ProjectTask_ProjectTaskId",
                table: "ProjectTaskFixCosts",
                column: "ProjectTaskId",
                principalTable: "ProjectTask",
                principalColumn: "ProjectTaskId");
        }
    }
}
