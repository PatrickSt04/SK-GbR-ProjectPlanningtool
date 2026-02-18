using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAAS_Projectplanningtool.Migrations
{
    /// <inheritdoc />
    public partial class initialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

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
                name: "LicenseModel",
                columns: table => new
                {
                    LicenseId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LicenseName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenseModel", x => x.LicenseId);
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
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
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
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModificationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestModificationText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.AddressId);
                });

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
                });

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SectorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LicenseId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AddressId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DefaultWorkDaysJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultWorkingHoursJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModificationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestModificationText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                        name: "FK_Company_LicenseModel_LicenseId",
                        column: x => x.LicenseId,
                        principalTable: "LicenseModel",
                        principalColumn: "LicenseId");
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AddressId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModificationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestModificationText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteFlag = table.Column<bool>(type: "bit", nullable: false)
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
                    IdentityUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IdentityRoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HourlyRateGroupId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeleteFlag = table.Column<bool>(type: "bit", nullable: false),
                    EmployeeDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModificationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestModificationText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_Employee_Employee_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employee_Employee_LatestModifierId",
                        column: x => x.LatestModifierId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HolidayCalendarEntry",
                columns: table => new
                {
                    HolidayCalendarEntryId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    HolidayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    HolidayDate = table.Column<DateOnly>(type: "date", nullable: false),
                    HolidayType = table.Column<int>(type: "int", nullable: false),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModificationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestModificationText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteFlag = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HolidayCalendarEntry", x => x.HolidayCalendarEntryId);
                    table.ForeignKey(
                        name: "FK_HolidayCalendarEntry_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_HolidayCalendarEntry_Employee_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HolidayCalendarEntry_Employee_LatestModifierId",
                        column: x => x.LatestModifierId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HourlyRateGroup",
                columns: table => new
                {
                    HourlyRateGroupId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    HourlyRate = table.Column<float>(type: "real", nullable: false),
                    HourlyRateGroupName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModificationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestModificationText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteFlag = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                        name: "FK_HourlyRateGroup_Employee_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HourlyRateGroup_Employee_LatestModifierId",
                        column: x => x.LatestModifierId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Logfile",
                columns: table => new
                {
                    LogfileId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExceptionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExceptionMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExcecutingEmployeeId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    TimeOfException = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExceptionPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SerializedObject = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logfile", x => x.LogfileId);
                    table.ForeignKey(
                        name: "FK_Logfile_Employee_ExcecutingEmployeeId",
                        column: x => x.ExcecutingEmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                });

            migrationBuilder.CreateTable(
                name: "ProjectBudget",
                columns: table => new
                {
                    ProjectBudgetId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    InitialBudget = table.Column<double>(type: "float", nullable: false),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModificationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestModificationText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InitialAdditionalCosts = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InitialHRGPlannings = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                        name: "FK_ProjectBudget_Employee_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectBudget_Employee_LatestModifierId",
                        column: x => x.LatestModifierId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
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
                    ResponsiblePersonId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    StateId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModificationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestModificationText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DefaultWorkDays = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultWorkingHoursJson = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                        name: "FK_Project_Employee_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Project_Employee_LatestModifierId",
                        column: x => x.LatestModifierId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Project_Employee_ResponsiblePersonId",
                        column: x => x.ResponsiblePersonId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_Project_ProjectBudget_ProjectBudgetId",
                        column: x => x.ProjectBudgetId,
                        principalTable: "ProjectBudget",
                        principalColumn: "ProjectBudgetId");
                    table.ForeignKey(
                        name: "FK_Project_State_StateId",
                        column: x => x.StateId,
                        principalTable: "State",
                        principalColumn: "StateId");
                });

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

            migrationBuilder.CreateTable(
                name: "ProjectSection",
                columns: table => new
                {
                    ProjectSectionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProjectSectionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProjectId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ParentSectionId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    StateId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModificationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestModificationText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                        name: "FK_ProjectSection_Employee_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectSection_Employee_LatestModifierId",
                        column: x => x.LatestModifierId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectSection_ProjectSection_ParentSectionId",
                        column: x => x.ParentSectionId,
                        principalTable: "ProjectSection",
                        principalColumn: "ProjectSectionId");
                    table.ForeignKey(
                        name: "FK_ProjectSection_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId");
                    table.ForeignKey(
                        name: "FK_ProjectSection_State_StateId",
                        column: x => x.StateId,
                        principalTable: "State",
                        principalColumn: "StateId");
                });

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

            migrationBuilder.CreateTable(
                name: "ProjectTask",
                columns: table => new
                {
                    ProjectTaskId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProjectTaskName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProjectSectionId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    StateId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModificationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestModificationText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                        name: "FK_ProjectTask_Employee_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectTask_Employee_LatestModifierId",
                        column: x => x.LatestModifierId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectTask_ProjectSection_ProjectSectionId",
                        column: x => x.ProjectSectionId,
                        principalTable: "ProjectSection",
                        principalColumn: "ProjectSectionId");
                    table.ForeignKey(
                        name: "FK_ProjectTask_State_StateId",
                        column: x => x.StateId,
                        principalTable: "State",
                        principalColumn: "StateId");
                });

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

            migrationBuilder.CreateTable(
                name: "ProjectTaskCatalogTask",
                columns: table => new
                {
                    ProjectTaskCatalogTaskId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TaskName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProjectId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
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

            migrationBuilder.CreateTable(
                name: "ProjectTaskFixCosts",
                columns: table => new
                {
                    ProjectTaskFixCostsId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TaskId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FixCosts = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTaskFixCosts", x => x.ProjectTaskFixCostsId);
                    table.ForeignKey(
                        name: "FK_ProjectTaskFixCosts_ProjectTaskCatalogTask_TaskId",
                        column: x => x.TaskId,
                        principalTable: "ProjectTaskCatalogTask",
                        principalColumn: "ProjectTaskCatalogTaskId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_CompanyId",
                table: "Address",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Address_CreatedById",
                table: "Address",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Address_LatestModifierId",
                table: "Address",
                column: "LatestModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

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

            migrationBuilder.CreateIndex(
                name: "IX_Company_AddressId",
                table: "Company",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Company_CreatedById",
                table: "Company",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Company_LatestModifierId",
                table: "Company",
                column: "LatestModifierId");

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
                name: "IX_Customer_CreatedById",
                table: "Customer",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_LatestModifierId",
                table: "Customer",
                column: "LatestModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_CompanyId",
                table: "Employee",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_CreatedById",
                table: "Employee",
                column: "CreatedById");

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
                name: "IX_Employee_LatestModifierId",
                table: "Employee",
                column: "LatestModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_HolidayCalendarEntry_CompanyId_HolidayDate",
                table: "HolidayCalendarEntry",
                columns: new[] { "CompanyId", "HolidayDate" });

            migrationBuilder.CreateIndex(
                name: "IX_HolidayCalendarEntry_CreatedById",
                table: "HolidayCalendarEntry",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HolidayCalendarEntry_LatestModifierId",
                table: "HolidayCalendarEntry",
                column: "LatestModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_HourlyRateGroup_CompanyId",
                table: "HourlyRateGroup",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_HourlyRateGroup_CreatedById",
                table: "HourlyRateGroup",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HourlyRateGroup_LatestModifierId",
                table: "HourlyRateGroup",
                column: "LatestModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_Logfile_ExcecutingEmployeeId",
                table: "Logfile",
                column: "ExcecutingEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_CompanyId",
                table: "Project",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_CreatedById",
                table: "Project",
                column: "CreatedById");

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
                name: "IX_Project_ResponsiblePersonId",
                table: "Project",
                column: "ResponsiblePersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_StateId",
                table: "Project",
                column: "StateId");

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

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBudget_CompanyId",
                table: "ProjectBudget",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBudget_CreatedById",
                table: "ProjectBudget",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBudget_LatestModifierId",
                table: "ProjectBudget",
                column: "LatestModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSection_CompanyId",
                table: "ProjectSection",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSection_CreatedById",
                table: "ProjectSection",
                column: "CreatedById");

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
                name: "IX_ProjectSection_StateId",
                table: "ProjectSection",
                column: "StateId");

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

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTask_CompanyId",
                table: "ProjectTask",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTask_CreatedById",
                table: "ProjectTask",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTask_LatestModifierId",
                table: "ProjectTask",
                column: "LatestModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTask_ProjectSectionId",
                table: "ProjectTask",
                column: "ProjectSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTask_StateId",
                table: "ProjectTask",
                column: "StateId");

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

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskFixCosts_TaskId",
                table: "ProjectTaskFixCosts",
                column: "TaskId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskHourlyRateGroup_HourlyRateGroupId",
                table: "ProjectTaskHourlyRateGroup",
                column: "HourlyRateGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskHourlyRateGroup_ProjectTaskId",
                table: "ProjectTaskHourlyRateGroup",
                column: "ProjectTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Company_CompanyId",
                table: "Address",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "CompanyId");

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
                name: "FK_BudgetRecalculation_Company_CompanyId",
                table: "BudgetRecalculation",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetRecalculation_Employee_RecalculatedById",
                table: "BudgetRecalculation",
                column: "RecalculatedById",
                principalTable: "Employee",
                principalColumn: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetRecalculation_ProjectBudget_ProjectBudgetId",
                table: "BudgetRecalculation",
                column: "ProjectBudgetId",
                principalTable: "ProjectBudget",
                principalColumn: "ProjectBudgetId");

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
                name: "FK_Employee_HourlyRateGroup_HourlyRateGroupId",
                table: "Employee",
                column: "HourlyRateGroupId",
                principalTable: "HourlyRateGroup",
                principalColumn: "HourlyRateGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTaskCatalogTask_ProjectTaskFixCosts_ProjectTaskFixCostsId",
                table: "ProjectTaskCatalogTask",
                column: "ProjectTaskFixCostsId",
                principalTable: "ProjectTaskFixCosts",
                principalColumn: "ProjectTaskFixCostsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_Company_CompanyId",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_Customer_Company_CompanyId",
                table: "Customer");

            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Company_CompanyId",
                table: "Employee");

            migrationBuilder.DropForeignKey(
                name: "FK_HourlyRateGroup_Company_CompanyId",
                table: "HourlyRateGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_Company_CompanyId",
                table: "Project");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectBudget_Company_CompanyId",
                table: "ProjectBudget");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTaskCatalogTask_Company_CompanyId",
                table: "ProjectTaskCatalogTask");

            migrationBuilder.DropForeignKey(
                name: "FK_Address_Employee_CreatedById",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_Address_Employee_LatestModifierId",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_Customer_Employee_CreatedById",
                table: "Customer");

            migrationBuilder.DropForeignKey(
                name: "FK_Customer_Employee_LatestModifierId",
                table: "Customer");

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
                name: "FK_Project_Employee_ResponsiblePersonId",
                table: "Project");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectBudget_Employee_CreatedById",
                table: "ProjectBudget");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectBudget_Employee_LatestModifierId",
                table: "ProjectBudget");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTaskCatalogTask_Employee_CreatedById",
                table: "ProjectTaskCatalogTask");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTaskCatalogTask_Employee_LatestModifierId",
                table: "ProjectTaskCatalogTask");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_ProjectBudget_ProjectBudgetId",
                table: "Project");

            migrationBuilder.DropForeignKey(
                name: "FK_Customer_Address_AddressId",
                table: "Customer");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_Customer_CustomerId",
                table: "Project");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_State_StateId",
                table: "Project");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTaskCatalogTask_State_StateId",
                table: "ProjectTaskCatalogTask");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTaskCatalogTask_Project_ProjectId",
                table: "ProjectTaskCatalogTask");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTaskCatalogTask_ProjectTaskFixCosts_ProjectTaskFixCostsId",
                table: "ProjectTaskCatalogTask");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BudgetRecalculation");

            migrationBuilder.DropTable(
                name: "HolidayCalendarEntry");

            migrationBuilder.DropTable(
                name: "Logfile");

            migrationBuilder.DropTable(
                name: "ProjectAdditionalCosts");

            migrationBuilder.DropTable(
                name: "ProjectSectionMilestone");

            migrationBuilder.DropTable(
                name: "ProjectTaskHourlyRateGroup");

            migrationBuilder.DropTable(
                name: "ProjectTask");

            migrationBuilder.DropTable(
                name: "ProjectSection");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropTable(
                name: "IndustrySector");

            migrationBuilder.DropTable(
                name: "LicenseModel");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "HourlyRateGroup");

            migrationBuilder.DropTable(
                name: "ProjectBudget");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "State");

            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.DropTable(
                name: "ProjectTaskFixCosts");

            migrationBuilder.DropTable(
                name: "ProjectTaskCatalogTask");
        }
    }
}
