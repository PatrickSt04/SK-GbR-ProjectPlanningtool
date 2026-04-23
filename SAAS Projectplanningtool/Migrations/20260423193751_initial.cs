using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAAS_Projectplanningtool.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
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
                name: "Article",
                columns: table => new
                {
                    ArticleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ArticleNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArticleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArticleCategoryId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UnitId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "bit", nullable: false),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModificationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestModificationText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Article", x => x.ArticleId);
                });

            migrationBuilder.CreateTable(
                name: "ArticleCategory",
                columns: table => new
                {
                    ArticleCategoryId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteFlag = table.Column<bool>(type: "bit", nullable: false),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModificationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestModificationText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleCategory", x => x.ArticleCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "ArticlePriceHistory",
                columns: table => new
                {
                    ArticlePriceHistoryId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ArticleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticlePriceHistory", x => x.ArticlePriceHistoryId);
                    table.ForeignKey(
                        name: "FK_ArticlePriceHistory_Article_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Article",
                        principalColumn: "ArticleId");
                });

            migrationBuilder.CreateTable(
                name: "BankAccount",
                columns: table => new
                {
                    BankAccountId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AccountName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IBAN = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: false),
                    BIC = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AccountHolder = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModificationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestModificationText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteFlag = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccount", x => x.BankAccountId);
                });

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
                name: "Unit",
                columns: table => new
                {
                    UnitId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unit", x => x.UnitId);
                    table.ForeignKey(
                        name: "FK_Unit_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                });

            migrationBuilder.CreateTable(
                name: "CompanyUnit",
                columns: table => new
                {
                    CompanyUnitId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UnitId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyUnit", x => x.CompanyUnitId);
                    table.ForeignKey(
                        name: "FK_CompanyUnit_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_CompanyUnit_Unit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Unit",
                        principalColumn: "UnitId");
                });

            migrationBuilder.CreateTable(
                name: "ContactHistoryEntry",
                columns: table => new
                {
                    EntryId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContactType = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactHistoryEntry", x => x.EntryId);
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AddressId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerType = table.Column<int>(type: "int", nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                name: "CustomerContactPerson",
                columns: table => new
                {
                    ContactPersonId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerContactPerson", x => x.ContactPersonId);
                    table.ForeignKey(
                        name: "FK_CustomerContactPerson_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "CustomerId");
                });

            migrationBuilder.CreateTable(
                name: "CustomerMaterialSurcharge",
                columns: table => new
                {
                    SurchargeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaterialCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SurchargePercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerMaterialSurcharge", x => x.SurchargeId);
                    table.ForeignKey(
                        name: "FK_CustomerMaterialSurcharge_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "CustomerId");
                });

            migrationBuilder.CreateTable(
                name: "CustomerMessage",
                columns: table => new
                {
                    MessageId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AttachmentPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttachmentFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClosedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ClosedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerMessage", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_CustomerMessage_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "CustomerId");
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    EmployeeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IdentityUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
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
                    HourlyRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                    ProjectLeadId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    InstructorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    StateId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModificationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestModificationText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DefaultWorkDays = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultWorkingHoursJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                        name: "FK_Project_Employee_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_Project_Employee_LatestModifierId",
                        column: x => x.LatestModifierId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Project_Employee_ProjectLeadId",
                        column: x => x.ProjectLeadId,
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
                name: "ProjectEmployeeViewerShare",
                columns: table => new
                {
                    ProjectEmployeeShareId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProjectId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    EmployeeId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModificationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestModificationText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectEmployeeViewerShare", x => x.ProjectEmployeeShareId);
                    table.ForeignKey(
                        name: "FK_ProjectEmployeeViewerShare_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_ProjectEmployeeViewerShare_Employee_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_ProjectEmployeeViewerShare_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_ProjectEmployeeViewerShare_Employee_LatestModifierId",
                        column: x => x.LatestModifierId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_ProjectEmployeeViewerShare_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId");
                });

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
                    StateId = table.Column<string>(type: "nvarchar(450)", nullable: true)
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
                name: "TimeEntry",
                columns: table => new
                {
                    TimeEntryId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProjectId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmployeeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WorkDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    BreakMinutes = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModificationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestModificationText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeEntry", x => x.TimeEntryId);
                    table.ForeignKey(
                        name: "FK_TimeEntry_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_TimeEntry_Employee_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_TimeEntry_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_TimeEntry_Employee_LatestModifierId",
                        column: x => x.LatestModifierId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_TimeEntry_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId");
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
                name: "IX_Article_ArticleCategoryId",
                table: "Article",
                column: "ArticleCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Article_CompanyId",
                table: "Article",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Article_CreatedById",
                table: "Article",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Article_LatestModifierId",
                table: "Article",
                column: "LatestModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_Article_UnitId",
                table: "Article",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleCategory_CompanyId",
                table: "ArticleCategory",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleCategory_CreatedById",
                table: "ArticleCategory",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleCategory_LatestModifierId",
                table: "ArticleCategory",
                column: "LatestModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticlePriceHistory_ArticleId",
                table: "ArticlePriceHistory",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticlePriceHistory_CreatedById",
                table: "ArticlePriceHistory",
                column: "CreatedById");

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
                name: "IX_BankAccount_CompanyId",
                table: "BankAccount",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccount_CreatedById",
                table: "BankAccount",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccount_LatestModifierId",
                table: "BankAccount",
                column: "LatestModifierId");

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
                name: "IX_CompanyUnit_CompanyId",
                table: "CompanyUnit",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUnit_UnitId",
                table: "CompanyUnit",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactHistoryEntry_CreatedById",
                table: "ContactHistoryEntry",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ContactHistoryEntry_CustomerId",
                table: "ContactHistoryEntry",
                column: "CustomerId");

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
                name: "IX_CustomerContactPerson_CustomerId",
                table: "CustomerContactPerson",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerMaterialSurcharge_CustomerId",
                table: "CustomerMaterialSurcharge",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerMessage_ClosedById",
                table: "CustomerMessage",
                column: "ClosedById");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerMessage_CreatedById",
                table: "CustomerMessage",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerMessage_CustomerId",
                table: "CustomerMessage",
                column: "CustomerId");

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
                name: "IX_Project_InstructorId",
                table: "Project",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_LatestModifierId",
                table: "Project",
                column: "LatestModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_ProjectBudgetId",
                table: "Project",
                column: "ProjectBudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_ProjectLeadId",
                table: "Project",
                column: "ProjectLeadId");

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
                name: "IX_ProjectEmployeeViewerShare_CompanyId",
                table: "ProjectEmployeeViewerShare",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectEmployeeViewerShare_CreatedById",
                table: "ProjectEmployeeViewerShare",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectEmployeeViewerShare_EmployeeId",
                table: "ProjectEmployeeViewerShare",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectEmployeeViewerShare_LatestModifierId",
                table: "ProjectEmployeeViewerShare",
                column: "LatestModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectEmployeeViewerShare_ProjectId",
                table: "ProjectEmployeeViewerShare",
                column: "ProjectId");

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
                name: "IX_ProjectTaskCatalogTask_StateId",
                table: "ProjectTaskCatalogTask",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeEntry_CompanyId",
                table: "TimeEntry",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeEntry_CreatedById",
                table: "TimeEntry",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TimeEntry_EmployeeId",
                table: "TimeEntry",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeEntry_LatestModifierId",
                table: "TimeEntry",
                column: "LatestModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeEntry_ProjectId",
                table: "TimeEntry",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Unit_CompanyId",
                table: "Unit",
                column: "CompanyId");

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
                name: "FK_Article_ArticleCategory_ArticleCategoryId",
                table: "Article",
                column: "ArticleCategoryId",
                principalTable: "ArticleCategory",
                principalColumn: "ArticleCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Article_Company_CompanyId",
                table: "Article",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Article_Employee_CreatedById",
                table: "Article",
                column: "CreatedById",
                principalTable: "Employee",
                principalColumn: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Article_Employee_LatestModifierId",
                table: "Article",
                column: "LatestModifierId",
                principalTable: "Employee",
                principalColumn: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Article_Unit_UnitId",
                table: "Article",
                column: "UnitId",
                principalTable: "Unit",
                principalColumn: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleCategory_Company_CompanyId",
                table: "ArticleCategory",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleCategory_Employee_CreatedById",
                table: "ArticleCategory",
                column: "CreatedById",
                principalTable: "Employee",
                principalColumn: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleCategory_Employee_LatestModifierId",
                table: "ArticleCategory",
                column: "LatestModifierId",
                principalTable: "Employee",
                principalColumn: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticlePriceHistory_Employee_CreatedById",
                table: "ArticlePriceHistory",
                column: "CreatedById",
                principalTable: "Employee",
                principalColumn: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccount_Company_CompanyId",
                table: "BankAccount",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccount_Employee_CreatedById",
                table: "BankAccount",
                column: "CreatedById",
                principalTable: "Employee",
                principalColumn: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccount_Employee_LatestModifierId",
                table: "BankAccount",
                column: "LatestModifierId",
                principalTable: "Employee",
                principalColumn: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetGroup_Company_CompanyId",
                table: "BudgetGroup",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetGroup_Employee_CreatedById",
                table: "BudgetGroup",
                column: "CreatedById",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetGroup_Employee_LatestModifierId",
                table: "BudgetGroup",
                column: "LatestModifierId",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetGroup_ProjectBudget_ProjectBudgetId",
                table: "BudgetGroup",
                column: "ProjectBudgetId",
                principalTable: "ProjectBudget",
                principalColumn: "ProjectBudgetId");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetLineItem_Company_CompanyId",
                table: "BudgetLineItem",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetLineItem_Employee_CreatedById",
                table: "BudgetLineItem",
                column: "CreatedById",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetLineItem_Employee_LatestModifierId",
                table: "BudgetLineItem",
                column: "LatestModifierId",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetLineItem_ProjectHourlyRateGroup_ProjectHourlyRateGroupId",
                table: "BudgetLineItem",
                column: "ProjectHourlyRateGroupId",
                principalTable: "ProjectHourlyRateGroup",
                principalColumn: "ProjectHourlyRateGroupId");

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
                name: "FK_ContactHistoryEntry_Customer_CustomerId",
                table: "ContactHistoryEntry",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactHistoryEntry_Employee_CreatedById",
                table: "ContactHistoryEntry",
                column: "CreatedById",
                principalTable: "Employee",
                principalColumn: "EmployeeId");

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
                name: "FK_CustomerMessage_Employee_ClosedById",
                table: "CustomerMessage",
                column: "ClosedById",
                principalTable: "Employee",
                principalColumn: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerMessage_Employee_CreatedById",
                table: "CustomerMessage",
                column: "CreatedById",
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
                name: "FK_Address_Company_CompanyId",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Company_CompanyId",
                table: "Employee");

            migrationBuilder.DropForeignKey(
                name: "FK_HourlyRateGroup_Company_CompanyId",
                table: "HourlyRateGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_HourlyRateGroup_Employee_CreatedById",
                table: "HourlyRateGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_HourlyRateGroup_Employee_LatestModifierId",
                table: "HourlyRateGroup");

            migrationBuilder.DropTable(
                name: "ArticlePriceHistory");

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
                name: "BankAccount");

            migrationBuilder.DropTable(
                name: "BudgetLineItem");

            migrationBuilder.DropTable(
                name: "BudgetRecalculation");

            migrationBuilder.DropTable(
                name: "CompanyUnit");

            migrationBuilder.DropTable(
                name: "ContactHistoryEntry");

            migrationBuilder.DropTable(
                name: "CustomerContactPerson");

            migrationBuilder.DropTable(
                name: "CustomerMaterialSurcharge");

            migrationBuilder.DropTable(
                name: "CustomerMessage");

            migrationBuilder.DropTable(
                name: "HolidayCalendarEntry");

            migrationBuilder.DropTable(
                name: "Logfile");

            migrationBuilder.DropTable(
                name: "ProjectAdditionalCosts");

            migrationBuilder.DropTable(
                name: "ProjectEmployeeViewerShare");

            migrationBuilder.DropTable(
                name: "ProjectSectionMilestone");

            migrationBuilder.DropTable(
                name: "ProjectTask");

            migrationBuilder.DropTable(
                name: "ProjectTaskCatalogTask");

            migrationBuilder.DropTable(
                name: "TimeEntry");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Article");

            migrationBuilder.DropTable(
                name: "BudgetGroup");

            migrationBuilder.DropTable(
                name: "ProjectHourlyRateGroup");

            migrationBuilder.DropTable(
                name: "ProjectSection");

            migrationBuilder.DropTable(
                name: "ArticleCategory");

            migrationBuilder.DropTable(
                name: "Unit");

            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "ProjectBudget");

            migrationBuilder.DropTable(
                name: "State");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "IndustrySector");

            migrationBuilder.DropTable(
                name: "LicenseModel");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "HourlyRateGroup");
        }
    }
}
