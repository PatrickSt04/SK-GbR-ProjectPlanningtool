using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAAS_Projectplanningtool.Migrations
{
    /// <inheritdoc />
    public partial class ProjectHRGs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectHourlyRateGroup",
                columns: table => new
                {
                    ProjectHourlyRateGroupId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProjectId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    HourlyRateGroupId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProjectHourlyRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModificationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestModificationText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectHourlyRateGroup", x => x.ProjectHourlyRateGroupId);
                    table.ForeignKey(
                        name: "FK_ProjectHourlyRateGroup_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_ProjectHourlyRateGroup_Employee_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_ProjectHourlyRateGroup_Employee_LatestModifierId",
                        column: x => x.LatestModifierId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_ProjectHourlyRateGroup_HourlyRateGroup_HourlyRateGroupId",
                        column: x => x.HourlyRateGroupId,
                        principalTable: "HourlyRateGroup",
                        principalColumn: "HourlyRateGroupId");
                    table.ForeignKey(
                        name: "FK_ProjectHourlyRateGroup_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectHourlyRateGroup_CompanyId",
                table: "ProjectHourlyRateGroup",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectHourlyRateGroup_CreatedById",
                table: "ProjectHourlyRateGroup",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectHourlyRateGroup_HourlyRateGroupId",
                table: "ProjectHourlyRateGroup",
                column: "HourlyRateGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectHourlyRateGroup_LatestModifierId",
                table: "ProjectHourlyRateGroup",
                column: "LatestModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectHourlyRateGroup_ProjectId",
                table: "ProjectHourlyRateGroup",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectHourlyRateGroup");
        }
    }
}
