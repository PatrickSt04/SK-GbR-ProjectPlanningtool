using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAAS_Projectplanningtool.Migrations
{
    /// <inheritdoc />
    public partial class ProjectTaskRess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTaskRessource_Ressource_RessourceId",
                table: "ProjectTaskRessource");

            migrationBuilder.DropTable(
                name: "Ressource");

            migrationBuilder.DropTable(
                name: "RessourceType");

            migrationBuilder.DropTable(
                name: "Unit");

            migrationBuilder.DropIndex(
                name: "IX_ProjectTaskRessource_RessourceId",
                table: "ProjectTaskRessource");

            migrationBuilder.DropColumn(
                name: "AmountPerUnit",
                table: "ProjectTaskRessource");

            migrationBuilder.DropColumn(
                name: "RessourceId",
                table: "ProjectTaskRessource");

            migrationBuilder.AddColumn<string>(
                name: "ProjectTaskName",
                table: "ProjectTaskRessource",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectTaskRessourceId",
                table: "Employee",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employee_ProjectTaskRessourceId",
                table: "Employee",
                column: "ProjectTaskRessourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_ProjectTaskRessource_ProjectTaskRessourceId",
                table: "Employee",
                column: "ProjectTaskRessourceId",
                principalTable: "ProjectTaskRessource",
                principalColumn: "ProjectTaskRessourceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_ProjectTaskRessource_ProjectTaskRessourceId",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Employee_ProjectTaskRessourceId",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "ProjectTaskName",
                table: "ProjectTaskRessource");

            migrationBuilder.DropColumn(
                name: "ProjectTaskRessourceId",
                table: "Employee");

            migrationBuilder.AddColumn<float>(
                name: "AmountPerUnit",
                table: "ProjectTaskRessource",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "RessourceId",
                table: "ProjectTaskRessource",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RessourceType",
                columns: table => new
                {
                    RessourceTypeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsEmployeeRessourceType = table.Column<bool>(type: "bit", nullable: false),
                    LatestModificationText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LatestModificationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RessourceTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RessourceType", x => x.RessourceTypeId);
                    table.ForeignKey(
                        name: "FK_RessourceType_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_RessourceType_Employee_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RessourceType_Employee_LatestModifierId",
                        column: x => x.LatestModifierId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Unit",
                columns: table => new
                {
                    UnitId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestModificationText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LatestModificationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unit", x => x.UnitId);
                    table.ForeignKey(
                        name: "FK_Unit_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_Unit_Employee_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Unit_Employee_LatestModifierId",
                        column: x => x.LatestModifierId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ressource",
                columns: table => new
                {
                    RessourceId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RessourceTypeId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UnitId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CostsPerUnit = table.Column<float>(type: "real", nullable: false),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestModificationText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LatestModificationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RessourceName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ressource", x => x.RessourceId);
                    table.ForeignKey(
                        name: "FK_Ressource_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_Ressource_Employee_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ressource_Employee_LatestModifierId",
                        column: x => x.LatestModifierId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ressource_RessourceType_RessourceTypeId",
                        column: x => x.RessourceTypeId,
                        principalTable: "RessourceType",
                        principalColumn: "RessourceTypeId");
                    table.ForeignKey(
                        name: "FK_Ressource_Unit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Unit",
                        principalColumn: "UnitId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskRessource_RessourceId",
                table: "ProjectTaskRessource",
                column: "RessourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Ressource_CompanyId",
                table: "Ressource",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Ressource_CreatedById",
                table: "Ressource",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Ressource_LatestModifierId",
                table: "Ressource",
                column: "LatestModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_Ressource_RessourceTypeId",
                table: "Ressource",
                column: "RessourceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Ressource_UnitId",
                table: "Ressource",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_RessourceType_CompanyId",
                table: "RessourceType",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_RessourceType_CreatedById",
                table: "RessourceType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RessourceType_LatestModifierId",
                table: "RessourceType",
                column: "LatestModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_Unit_CompanyId",
                table: "Unit",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Unit_CreatedById",
                table: "Unit",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Unit_LatestModifierId",
                table: "Unit",
                column: "LatestModifierId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTaskRessource_Ressource_RessourceId",
                table: "ProjectTaskRessource",
                column: "RessourceId",
                principalTable: "Ressource",
                principalColumn: "RessourceId");
        }
    }
}
