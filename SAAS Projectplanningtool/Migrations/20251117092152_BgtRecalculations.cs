using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAAS_Projectplanningtool.Migrations
{
    /// <inheritdoc />
    public partial class BgtRecalculations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BudgetRecalculation",
                columns: table => new
                {
                    RecalculationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NewBudget = table.Column<double>(type: "float", nullable: false),
                    RecalculationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecalculatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProjectBudgetId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetRecalculation", x => x.RecalculationId);
                    table.ForeignKey(
                        name: "FK_BudgetRecalculation_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_BudgetRecalculation_Employee_RecalculatedById",
                        column: x => x.RecalculatedById,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_BudgetRecalculation_ProjectBudget_ProjectBudgetId",
                        column: x => x.ProjectBudgetId,
                        principalTable: "ProjectBudget",
                        principalColumn: "ProjectBudgetId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetRecalculation_CompanyId",
                table: "BudgetRecalculation",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetRecalculation_ProjectBudgetId",
                table: "BudgetRecalculation",
                column: "ProjectBudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetRecalculation_RecalculatedById",
                table: "BudgetRecalculation",
                column: "RecalculatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetRecalculation");
        }
    }
}
