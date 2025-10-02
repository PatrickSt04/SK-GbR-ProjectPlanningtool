using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAAS_Projectplanningtool.Migrations
{
    /// <inheritdoc />
    public partial class PAC_Edited : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectAdditionalCosts",
                columns: table => new
                {
                    ProjectAdditionalCostsId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProjectId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AdditionalCostName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdditionalCostAmount = table.Column<double>(type: "float", nullable: true),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModificationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestModificationText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectAdditionalCosts", x => x.ProjectAdditionalCostsId);
                    table.ForeignKey(
                        name: "FK_ProjectAdditionalCosts_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_ProjectAdditionalCosts_Employee_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_ProjectAdditionalCosts_Employee_LatestModifierId",
                        column: x => x.LatestModifierId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_ProjectAdditionalCosts_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAdditionalCosts_CompanyId",
                table: "ProjectAdditionalCosts",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAdditionalCosts_CreatedById",
                table: "ProjectAdditionalCosts",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAdditionalCosts_LatestModifierId",
                table: "ProjectAdditionalCosts",
                column: "LatestModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAdditionalCosts_ProjectId",
                table: "ProjectAdditionalCosts",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectAdditionalCosts");
        }
    }
}
