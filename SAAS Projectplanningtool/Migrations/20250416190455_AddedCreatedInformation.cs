using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAAS_Projectplanningtool.Migrations
{
    /// <inheritdoc />
    public partial class AddedCreatedInformation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_Employee_LatestModifierId",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_Company_Employee_LatestModifierId",
                table: "Company");

            migrationBuilder.DropForeignKey(
                name: "FK_Customer_Employee_LatestModifierId",
                table: "Customer");

            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Employee_LatestModifierId",
                table: "Employee");

            migrationBuilder.DropForeignKey(
                name: "FK_HourlyRateGroup_Employee_LatestModifierId",
                table: "HourlyRateGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_Employee_LatestModifierId",
                table: "Project");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectBudget_Employee_LatestModifierId",
                table: "ProjectBudget");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectSection_Employee_LatestModifierId",
                table: "ProjectSection");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTask_Employee_LatestModifierId",
                table: "ProjectTask");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTaskRessource_Employee_LatestModifierId",
                table: "ProjectTaskRessource");

            migrationBuilder.DropForeignKey(
                name: "FK_Ressource_Employee_LatestModifierId",
                table: "Ressource");

            migrationBuilder.DropForeignKey(
                name: "FK_RessourceType_Employee_LatestModifierId",
                table: "RessourceType");

            migrationBuilder.DropForeignKey(
                name: "FK_Unit_Employee_LatestModifierId",
                table: "Unit");

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Unit",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTimestamp",
                table: "Unit",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "RessourceType",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTimestamp",
                table: "RessourceType",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Ressource",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTimestamp",
                table: "Ressource",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "ProjectTaskRessource",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTimestamp",
                table: "ProjectTaskRessource",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "ProjectTask",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTimestamp",
                table: "ProjectTask",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "ProjectSection",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTimestamp",
                table: "ProjectSection",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "ProjectBudget",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTimestamp",
                table: "ProjectBudget",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Project",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTimestamp",
                table: "Project",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "HourlyRateGroup",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTimestamp",
                table: "HourlyRateGroup",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Employee",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTimestamp",
                table: "Employee",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Customer",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTimestamp",
                table: "Customer",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Company",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTimestamp",
                table: "Company",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Address",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTimestamp",
                table: "Address",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Unit_CreatedById",
                table: "Unit",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RessourceType_CreatedById",
                table: "RessourceType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Ressource_CreatedById",
                table: "Ressource",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskRessource_CreatedById",
                table: "ProjectTaskRessource",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTask_CreatedById",
                table: "ProjectTask",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSection_CreatedById",
                table: "ProjectSection",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBudget_CreatedById",
                table: "ProjectBudget",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Project_CreatedById",
                table: "Project",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HourlyRateGroup_CreatedById",
                table: "HourlyRateGroup",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_CreatedById",
                table: "Employee",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_CreatedById",
                table: "Customer",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Company_CreatedById",
                table: "Company",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Address_CreatedById",
                table: "Address",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Employee_CreatedById",
                table: "Address",
                column: "CreatedById",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Employee_LatestModifierId",
                table: "Address",
                column: "LatestModifierId",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Company_Employee_CreatedById",
                table: "Company",
                column: "CreatedById",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Company_Employee_LatestModifierId",
                table: "Company",
                column: "LatestModifierId",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_Employee_CreatedById",
                table: "Customer",
                column: "CreatedById",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_Employee_LatestModifierId",
                table: "Customer",
                column: "LatestModifierId",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Employee_CreatedById",
                table: "Employee",
                column: "CreatedById",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Employee_LatestModifierId",
                table: "Employee",
                column: "LatestModifierId",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HourlyRateGroup_Employee_CreatedById",
                table: "HourlyRateGroup",
                column: "CreatedById",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HourlyRateGroup_Employee_LatestModifierId",
                table: "HourlyRateGroup",
                column: "LatestModifierId",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Project_Employee_CreatedById",
                table: "Project",
                column: "CreatedById",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Project_Employee_LatestModifierId",
                table: "Project",
                column: "LatestModifierId",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectBudget_Employee_CreatedById",
                table: "ProjectBudget",
                column: "CreatedById",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectBudget_Employee_LatestModifierId",
                table: "ProjectBudget",
                column: "LatestModifierId",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectSection_Employee_CreatedById",
                table: "ProjectSection",
                column: "CreatedById",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectSection_Employee_LatestModifierId",
                table: "ProjectSection",
                column: "LatestModifierId",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTask_Employee_CreatedById",
                table: "ProjectTask",
                column: "CreatedById",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTask_Employee_LatestModifierId",
                table: "ProjectTask",
                column: "LatestModifierId",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTaskRessource_Employee_CreatedById",
                table: "ProjectTaskRessource",
                column: "CreatedById",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTaskRessource_Employee_LatestModifierId",
                table: "ProjectTaskRessource",
                column: "LatestModifierId",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ressource_Employee_CreatedById",
                table: "Ressource",
                column: "CreatedById",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ressource_Employee_LatestModifierId",
                table: "Ressource",
                column: "LatestModifierId",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RessourceType_Employee_CreatedById",
                table: "RessourceType",
                column: "CreatedById",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RessourceType_Employee_LatestModifierId",
                table: "RessourceType",
                column: "LatestModifierId",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Unit_Employee_CreatedById",
                table: "Unit",
                column: "CreatedById",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Unit_Employee_LatestModifierId",
                table: "Unit",
                column: "LatestModifierId",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_Employee_CreatedById",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_Address_Employee_LatestModifierId",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_Company_Employee_CreatedById",
                table: "Company");

            migrationBuilder.DropForeignKey(
                name: "FK_Company_Employee_LatestModifierId",
                table: "Company");

            migrationBuilder.DropForeignKey(
                name: "FK_Customer_Employee_CreatedById",
                table: "Customer");

            migrationBuilder.DropForeignKey(
                name: "FK_Customer_Employee_LatestModifierId",
                table: "Customer");

            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Employee_CreatedById",
                table: "Employee");

            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Employee_LatestModifierId",
                table: "Employee");

            migrationBuilder.DropForeignKey(
                name: "FK_HourlyRateGroup_Employee_CreatedById",
                table: "HourlyRateGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_HourlyRateGroup_Employee_LatestModifierId",
                table: "HourlyRateGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_Employee_CreatedById",
                table: "Project");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_Employee_LatestModifierId",
                table: "Project");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectBudget_Employee_CreatedById",
                table: "ProjectBudget");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectBudget_Employee_LatestModifierId",
                table: "ProjectBudget");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectSection_Employee_CreatedById",
                table: "ProjectSection");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectSection_Employee_LatestModifierId",
                table: "ProjectSection");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTask_Employee_CreatedById",
                table: "ProjectTask");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTask_Employee_LatestModifierId",
                table: "ProjectTask");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTaskRessource_Employee_CreatedById",
                table: "ProjectTaskRessource");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTaskRessource_Employee_LatestModifierId",
                table: "ProjectTaskRessource");

            migrationBuilder.DropForeignKey(
                name: "FK_Ressource_Employee_CreatedById",
                table: "Ressource");

            migrationBuilder.DropForeignKey(
                name: "FK_Ressource_Employee_LatestModifierId",
                table: "Ressource");

            migrationBuilder.DropForeignKey(
                name: "FK_RessourceType_Employee_CreatedById",
                table: "RessourceType");

            migrationBuilder.DropForeignKey(
                name: "FK_RessourceType_Employee_LatestModifierId",
                table: "RessourceType");

            migrationBuilder.DropForeignKey(
                name: "FK_Unit_Employee_CreatedById",
                table: "Unit");

            migrationBuilder.DropForeignKey(
                name: "FK_Unit_Employee_LatestModifierId",
                table: "Unit");

            migrationBuilder.DropIndex(
                name: "IX_Unit_CreatedById",
                table: "Unit");

            migrationBuilder.DropIndex(
                name: "IX_RessourceType_CreatedById",
                table: "RessourceType");

            migrationBuilder.DropIndex(
                name: "IX_Ressource_CreatedById",
                table: "Ressource");

            migrationBuilder.DropIndex(
                name: "IX_ProjectTaskRessource_CreatedById",
                table: "ProjectTaskRessource");

            migrationBuilder.DropIndex(
                name: "IX_ProjectTask_CreatedById",
                table: "ProjectTask");

            migrationBuilder.DropIndex(
                name: "IX_ProjectSection_CreatedById",
                table: "ProjectSection");

            migrationBuilder.DropIndex(
                name: "IX_ProjectBudget_CreatedById",
                table: "ProjectBudget");

            migrationBuilder.DropIndex(
                name: "IX_Project_CreatedById",
                table: "Project");

            migrationBuilder.DropIndex(
                name: "IX_HourlyRateGroup_CreatedById",
                table: "HourlyRateGroup");

            migrationBuilder.DropIndex(
                name: "IX_Employee_CreatedById",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Customer_CreatedById",
                table: "Customer");

            migrationBuilder.DropIndex(
                name: "IX_Company_CreatedById",
                table: "Company");

            migrationBuilder.DropIndex(
                name: "IX_Address_CreatedById",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Unit");

            migrationBuilder.DropColumn(
                name: "CreatedTimestamp",
                table: "Unit");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "RessourceType");

            migrationBuilder.DropColumn(
                name: "CreatedTimestamp",
                table: "RessourceType");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Ressource");

            migrationBuilder.DropColumn(
                name: "CreatedTimestamp",
                table: "Ressource");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "ProjectTaskRessource");

            migrationBuilder.DropColumn(
                name: "CreatedTimestamp",
                table: "ProjectTaskRessource");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "ProjectTask");

            migrationBuilder.DropColumn(
                name: "CreatedTimestamp",
                table: "ProjectTask");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "ProjectSection");

            migrationBuilder.DropColumn(
                name: "CreatedTimestamp",
                table: "ProjectSection");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "ProjectBudget");

            migrationBuilder.DropColumn(
                name: "CreatedTimestamp",
                table: "ProjectBudget");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "CreatedTimestamp",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "HourlyRateGroup");

            migrationBuilder.DropColumn(
                name: "CreatedTimestamp",
                table: "HourlyRateGroup");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "CreatedTimestamp",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "CreatedTimestamp",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "CreatedTimestamp",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "CreatedTimestamp",
                table: "Address");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Employee_LatestModifierId",
                table: "Address",
                column: "LatestModifierId",
                principalTable: "Employee",
                principalColumn: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Company_Employee_LatestModifierId",
                table: "Company",
                column: "LatestModifierId",
                principalTable: "Employee",
                principalColumn: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_Employee_LatestModifierId",
                table: "Customer",
                column: "LatestModifierId",
                principalTable: "Employee",
                principalColumn: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Employee_LatestModifierId",
                table: "Employee",
                column: "LatestModifierId",
                principalTable: "Employee",
                principalColumn: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_HourlyRateGroup_Employee_LatestModifierId",
                table: "HourlyRateGroup",
                column: "LatestModifierId",
                principalTable: "Employee",
                principalColumn: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_Employee_LatestModifierId",
                table: "Project",
                column: "LatestModifierId",
                principalTable: "Employee",
                principalColumn: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectBudget_Employee_LatestModifierId",
                table: "ProjectBudget",
                column: "LatestModifierId",
                principalTable: "Employee",
                principalColumn: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectSection_Employee_LatestModifierId",
                table: "ProjectSection",
                column: "LatestModifierId",
                principalTable: "Employee",
                principalColumn: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTask_Employee_LatestModifierId",
                table: "ProjectTask",
                column: "LatestModifierId",
                principalTable: "Employee",
                principalColumn: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTaskRessource_Employee_LatestModifierId",
                table: "ProjectTaskRessource",
                column: "LatestModifierId",
                principalTable: "Employee",
                principalColumn: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ressource_Employee_LatestModifierId",
                table: "Ressource",
                column: "LatestModifierId",
                principalTable: "Employee",
                principalColumn: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_RessourceType_Employee_LatestModifierId",
                table: "RessourceType",
                column: "LatestModifierId",
                principalTable: "Employee",
                principalColumn: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Unit_Employee_LatestModifierId",
                table: "Unit",
                column: "LatestModifierId",
                principalTable: "Employee",
                principalColumn: "EmployeeId");
        }
    }
}
