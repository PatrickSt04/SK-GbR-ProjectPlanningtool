using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAAS_Projectplanningtool.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.CreateTable(
                name: "IndustrySector",
                columns: table => new
                {
                    SectorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SectorName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndustrySector", x => x.SectorId);
                });

            migrationBuilder.CreateTable(
                name: "License",
                columns: table => new
                {
                    LicenseId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LicenseName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_License", x => x.LicenseId);
                });

            migrationBuilder.CreateTable(
                name: "State",
                columns: table => new
                {
                    StateId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StateName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_State", x => x.StateId);
                });

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    AddressId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HouseNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.AddressId);
                });

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SectorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LicenseId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AddressId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DefaultWorkDays = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.CompanyId);
                    table.ForeignKey(
                        name: "FK_Company_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "AddressId");
                    table.ForeignKey(
                        name: "FK_Company_IndustrySector_SectorId",
                        column: x => x.SectorId,
                        principalTable: "IndustrySector",
                        principalColumn: "SectorId");
                    table.ForeignKey(
                        name: "FK_Company_License_LicenseId",
                        column: x => x.LicenseId,
                        principalTable: "License",
                        principalColumn: "LicenseId");
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AddressId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.CustomerId);
                    table.ForeignKey(
                        name: "FK_Customer_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "AddressId");
                    table.ForeignKey(
                        name: "FK_Customer_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    EmployeeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IdentityUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdentityRoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HourlyRateGroupId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmployeeDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.EmployeeId);
                    table.ForeignKey(
                        name: "FK_Employee_AspNetRoles_IdentityRoleId",
                        column: x => x.IdentityRoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Employee_AspNetUsers_IdentityUserId",
                        column: x => x.IdentityUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Employee_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                });

            migrationBuilder.CreateTable(
                name: "HourlyRateGroup",
                columns: table => new
                {
                    HourlyRateGroupId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    HourlyRate = table.Column<float>(type: "real", nullable: false),
                    HourlyRateGroupName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HourlyRateGroup", x => x.HourlyRateGroupId);
                    table.ForeignKey(
                        name: "FK_HourlyRateGroup_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_HourlyRateGroup_Employee_LatestModifierId",
                        column: x => x.LatestModifierId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                });

            migrationBuilder.CreateTable(
                name: "ProjectBudget",
                columns: table => new
                {
                    ProjectBudgetId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    InitialBudget = table.Column<float>(type: "real", nullable: false),
                    UsedBudget = table.Column<float>(type: "real", nullable: false),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectBudget", x => x.ProjectBudgetId);
                    table.ForeignKey(
                        name: "FK_ProjectBudget_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_ProjectBudget_Employee_LatestModifierId",
                        column: x => x.LatestModifierId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                });

            migrationBuilder.CreateTable(
                name: "RessourceType",
                columns: table => new
                {
                    RessourceTypeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RessourceTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsEmployeeRessourceType = table.Column<bool>(type: "bit", nullable: false),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true)
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
                        name: "FK_RessourceType_Employee_LatestModifierId",
                        column: x => x.LatestModifierId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                });

            migrationBuilder.CreateTable(
                name: "Unit",
                columns: table => new
                {
                    UnitId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UnitName = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                        name: "FK_Unit_Employee_LatestModifierId",
                        column: x => x.LatestModifierId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                });

            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    ProjectId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProjectBudgetId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProjectName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjectDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsArchived = table.Column<bool>(type: "bit", nullable: true),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.ProjectId);
                    table.ForeignKey(
                        name: "FK_Project_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_Project_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "CustomerId");
                    table.ForeignKey(
                        name: "FK_Project_Employee_LatestModifierId",
                        column: x => x.LatestModifierId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_Project_ProjectBudget_ProjectBudgetId",
                        column: x => x.ProjectBudgetId,
                        principalTable: "ProjectBudget",
                        principalColumn: "ProjectBudgetId");
                });

            migrationBuilder.CreateTable(
                name: "Ressource",
                columns: table => new
                {
                    RessourceId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RessourceTypeId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UnitId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CostsPerUnit = table.Column<float>(type: "real", nullable: false),
                    RessourceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true)
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
                        name: "FK_Ressource_Employee_LatestModifierId",
                        column: x => x.LatestModifierId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
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

            migrationBuilder.CreateTable(
                name: "ProjectSection",
                columns: table => new
                {
                    ProjectSectionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProjectId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SubSectionId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ParentSectionId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectSection", x => x.ProjectSectionId);
                    table.ForeignKey(
                        name: "FK_ProjectSection_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_ProjectSection_Employee_LatestModifierId",
                        column: x => x.LatestModifierId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_ProjectSection_ProjectSection_ParentSectionId",
                        column: x => x.ParentSectionId,
                        principalTable: "ProjectSection",
                        principalColumn: "ProjectSectionId");
                    table.ForeignKey(
                        name: "FK_ProjectSection_ProjectSection_SubSectionId",
                        column: x => x.SubSectionId,
                        principalTable: "ProjectSection",
                        principalColumn: "ProjectSectionId");
                    table.ForeignKey(
                        name: "FK_ProjectSection_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId");
                });

            migrationBuilder.CreateTable(
                name: "ProjectTask",
                columns: table => new
                {
                    ProjectTaskId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProjectTaskName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProjectSectionID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTask", x => x.ProjectTaskId);
                    table.ForeignKey(
                        name: "FK_ProjectTask_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_ProjectTask_Employee_LatestModifierId",
                        column: x => x.LatestModifierId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_ProjectTask_ProjectSection_ProjectSectionID",
                        column: x => x.ProjectSectionID,
                        principalTable: "ProjectSection",
                        principalColumn: "ProjectSectionId");
                });

            migrationBuilder.CreateTable(
                name: "ProjectTaskRessource",
                columns: table => new
                {
                    ProjectTaskRessourceId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RessourceId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProjectTaskId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AmountPerUnit = table.Column<float>(type: "real", nullable: false),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true)
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
                        name: "FK_ProjectTaskRessource_Employee_LatestModifierId",
                        column: x => x.LatestModifierId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_ProjectTaskRessource_ProjectTask_ProjectTaskId",
                        column: x => x.ProjectTaskId,
                        principalTable: "ProjectTask",
                        principalColumn: "ProjectTaskId");
                    table.ForeignKey(
                        name: "FK_ProjectTaskRessource_Ressource_RessourceId",
                        column: x => x.RessourceId,
                        principalTable: "Ressource",
                        principalColumn: "RessourceId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_CompanyId",
                table: "Address",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Address_LatestModifierId",
                table: "Address",
                column: "LatestModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_Company_AddressId",
                table: "Company",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Company_LicenseId",
                table: "Company",
                column: "LicenseId");

            migrationBuilder.CreateIndex(
                name: "IX_Company_SectorId",
                table: "Company",
                column: "SectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_AddressId",
                table: "Customer",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_CompanyId",
                table: "Customer",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_LatestModifierId",
                table: "Customer",
                column: "LatestModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_CompanyId",
                table: "Employee",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_HourlyRateGroupId",
                table: "Employee",
                column: "HourlyRateGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_IdentityRoleId",
                table: "Employee",
                column: "IdentityRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_IdentityUserId",
                table: "Employee",
                column: "IdentityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_HourlyRateGroup_CompanyId",
                table: "HourlyRateGroup",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_HourlyRateGroup_LatestModifierId",
                table: "HourlyRateGroup",
                column: "LatestModifierId",
                unique: true,
                filter: "[LatestModifierId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Project_CompanyId",
                table: "Project",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_CustomerId",
                table: "Project",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_LatestModifierId",
                table: "Project",
                column: "LatestModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_ProjectBudgetId",
                table: "Project",
                column: "ProjectBudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBudget_CompanyId",
                table: "ProjectBudget",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBudget_LatestModifierId",
                table: "ProjectBudget",
                column: "LatestModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSection_CompanyId",
                table: "ProjectSection",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSection_LatestModifierId",
                table: "ProjectSection",
                column: "LatestModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSection_ParentSectionId",
                table: "ProjectSection",
                column: "ParentSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSection_ProjectId",
                table: "ProjectSection",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSection_SubSectionId",
                table: "ProjectSection",
                column: "SubSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTask_CompanyId",
                table: "ProjectTask",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTask_LatestModifierId",
                table: "ProjectTask",
                column: "LatestModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTask_ProjectSectionID",
                table: "ProjectTask",
                column: "ProjectSectionID");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskRessource_CompanyId",
                table: "ProjectTaskRessource",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskRessource_LatestModifierId",
                table: "ProjectTaskRessource",
                column: "LatestModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskRessource_ProjectTaskId",
                table: "ProjectTaskRessource",
                column: "ProjectTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskRessource_RessourceId",
                table: "ProjectTaskRessource",
                column: "RessourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Ressource_CompanyId",
                table: "Ressource",
                column: "CompanyId");

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
                name: "IX_RessourceType_LatestModifierId",
                table: "RessourceType",
                column: "LatestModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_Unit_CompanyId",
                table: "Unit",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Unit_LatestModifierId",
                table: "Unit",
                column: "LatestModifierId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Company_CompanyId",
                table: "Address",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Employee_LatestModifierId",
                table: "Address",
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
                name: "FK_Employee_HourlyRateGroup_HourlyRateGroupId",
                table: "Employee",
                column: "HourlyRateGroupId",
                principalTable: "HourlyRateGroup",
                principalColumn: "HourlyRateGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_Address_Company_CompanyId",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Company_CompanyId",
                table: "Employee");

            migrationBuilder.DropForeignKey(
                name: "FK_HourlyRateGroup_Company_CompanyId",
                table: "HourlyRateGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_HourlyRateGroup_Employee_LatestModifierId",
                table: "HourlyRateGroup");

            migrationBuilder.DropTable(
                name: "ProjectTaskRessource");

            migrationBuilder.DropTable(
                name: "State");

            migrationBuilder.DropTable(
                name: "ProjectTask");

            migrationBuilder.DropTable(
                name: "Ressource");

            migrationBuilder.DropTable(
                name: "ProjectSection");

            migrationBuilder.DropTable(
                name: "RessourceType");

            migrationBuilder.DropTable(
                name: "Unit");

            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "ProjectBudget");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "IndustrySector");

            migrationBuilder.DropTable(
                name: "License");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "HourlyRateGroup");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
