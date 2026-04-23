using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAAS_Projectplanningtool.Migrations
{
    /// <inheritdoc />
    public partial class BudgetLineItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BudgetGroup",
                columns: table => new
                {
                    BudgetGroupId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProjectBudgetId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModificationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestModificationText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetGroup", x => x.BudgetGroupId);
                    table.ForeignKey(
                        name: "FK_BudgetGroup_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_BudgetGroup_Employee_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetGroup_Employee_LatestModifierId",
                        column: x => x.LatestModifierId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetGroup_ProjectBudget_ProjectBudgetId",
                        column: x => x.ProjectBudgetId,
                        principalTable: "ProjectBudget",
                        principalColumn: "ProjectBudgetId");
                });

            migrationBuilder.CreateTable(
                name: "BudgetLineItem",
                columns: table => new
                {
                    BudgetLineItemId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    BudgetGroupId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    LineItemType = table.Column<int>(type: "int", nullable: false),
                    ArticleId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProjectHourlyRateGroupId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AdjustedPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModificationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestModificationText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetLineItem", x => x.BudgetLineItemId);
                    table.ForeignKey(
                        name: "FK_BudgetLineItem_Article_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Article",
                        principalColumn: "ArticleId");
                    table.ForeignKey(
                        name: "FK_BudgetLineItem_BudgetGroup_BudgetGroupId",
                        column: x => x.BudgetGroupId,
                        principalTable: "BudgetGroup",
                        principalColumn: "BudgetGroupId");
                    table.ForeignKey(
                        name: "FK_BudgetLineItem_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_BudgetLineItem_Employee_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetLineItem_Employee_LatestModifierId",
                        column: x => x.LatestModifierId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetLineItem_ProjectHourlyRateGroup_ProjectHourlyRateGroupId",
                        column: x => x.ProjectHourlyRateGroupId,
                        principalTable: "ProjectHourlyRateGroup",
                        principalColumn: "ProjectHourlyRateGroupId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetGroup_CompanyId",
                table: "BudgetGroup",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetGroup_CreatedById",
                table: "BudgetGroup",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetGroup_LatestModifierId",
                table: "BudgetGroup",
                column: "LatestModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetGroup_ProjectBudgetId",
                table: "BudgetGroup",
                column: "ProjectBudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetLineItem_ArticleId",
                table: "BudgetLineItem",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetLineItem_BudgetGroupId",
                table: "BudgetLineItem",
                column: "BudgetGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetLineItem_CompanyId",
                table: "BudgetLineItem",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetLineItem_CreatedById",
                table: "BudgetLineItem",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetLineItem_LatestModifierId",
                table: "BudgetLineItem",
                column: "LatestModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetLineItem_ProjectHourlyRateGroupId",
                table: "BudgetLineItem",
                column: "ProjectHourlyRateGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetLineItem");

            migrationBuilder.DropTable(
                name: "BudgetGroup");
        }
    }
}
