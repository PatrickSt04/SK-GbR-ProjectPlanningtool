using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAAS_Projectplanningtool.Migrations
{
    /// <inheritdoc />
    public partial class DbTableMilestone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectSectionMilestone",
                columns: table => new
                {
                    ProjectSectionMilestoneId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MilestoneName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProjectSectionId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: true),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModificationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestModificationText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectSectionMilestone", x => x.ProjectSectionMilestoneId);
                    table.ForeignKey(
                        name: "FK_ProjectSectionMilestone_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_ProjectSectionMilestone_Employee_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_ProjectSectionMilestone_Employee_LatestModifierId",
                        column: x => x.LatestModifierId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_ProjectSectionMilestone_ProjectSection_ProjectSectionId",
                        column: x => x.ProjectSectionId,
                        principalTable: "ProjectSection",
                        principalColumn: "ProjectSectionId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSectionMilestone_CompanyId",
                table: "ProjectSectionMilestone",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSectionMilestone_CreatedById",
                table: "ProjectSectionMilestone",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSectionMilestone_LatestModifierId",
                table: "ProjectSectionMilestone",
                column: "LatestModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSectionMilestone_ProjectSectionId",
                table: "ProjectSectionMilestone",
                column: "ProjectSectionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectSectionMilestone");
        }
    }
}
