using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAAS_Projectplanningtool.Migrations
{
    /// <inheritdoc />
    public partial class offer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Offer",
                columns: table => new
                {
                    OfferId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProjectId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OfferName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OfferDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(5,4)", nullable: false),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModificationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestModificationText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offer", x => x.OfferId);
                    table.ForeignKey(
                        name: "FK_Offer_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_Offer_Employee_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Offer_Employee_LatestModifierId",
                        column: x => x.LatestModifierId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Offer_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId");
                });

            migrationBuilder.CreateTable(
                name: "OfferGroup",
                columns: table => new
                {
                    OfferGroupId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    OfferId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsHidden = table.Column<bool>(type: "bit", nullable: false),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModificationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestModificationText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferGroup", x => x.OfferGroupId);
                    table.ForeignKey(
                        name: "FK_OfferGroup_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_OfferGroup_Employee_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferGroup_Employee_LatestModifierId",
                        column: x => x.LatestModifierId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferGroup_Offer_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offer",
                        principalColumn: "OfferId");
                });

            migrationBuilder.CreateTable(
                name: "OfferLineItem",
                columns: table => new
                {
                    OfferLineItemId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    OfferGroupId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    LineItemType = table.Column<int>(type: "int", nullable: false),
                    ArticleId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProjectHourlyRateGroupId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SnapshotArticleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AdjustedPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsHidden = table.Column<bool>(type: "bit", nullable: false),
                    LatestModifierId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LatestModificationTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestModificationText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferLineItem", x => x.OfferLineItemId);
                    table.ForeignKey(
                        name: "FK_OfferLineItem_Article_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Article",
                        principalColumn: "ArticleId");
                    table.ForeignKey(
                        name: "FK_OfferLineItem_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_OfferLineItem_Employee_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferLineItem_Employee_LatestModifierId",
                        column: x => x.LatestModifierId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OfferLineItem_OfferGroup_OfferGroupId",
                        column: x => x.OfferGroupId,
                        principalTable: "OfferGroup",
                        principalColumn: "OfferGroupId");
                    table.ForeignKey(
                        name: "FK_OfferLineItem_ProjectHourlyRateGroup_ProjectHourlyRateGroupId",
                        column: x => x.ProjectHourlyRateGroupId,
                        principalTable: "ProjectHourlyRateGroup",
                        principalColumn: "ProjectHourlyRateGroupId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Offer_CompanyId",
                table: "Offer",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_CreatedById",
                table: "Offer",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_LatestModifierId",
                table: "Offer",
                column: "LatestModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_ProjectId",
                table: "Offer",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferGroup_CompanyId",
                table: "OfferGroup",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferGroup_CreatedById",
                table: "OfferGroup",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OfferGroup_LatestModifierId",
                table: "OfferGroup",
                column: "LatestModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferGroup_OfferId",
                table: "OfferGroup",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferLineItem_ArticleId",
                table: "OfferLineItem",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferLineItem_CompanyId",
                table: "OfferLineItem",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferLineItem_CreatedById",
                table: "OfferLineItem",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OfferLineItem_LatestModifierId",
                table: "OfferLineItem",
                column: "LatestModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferLineItem_OfferGroupId",
                table: "OfferLineItem",
                column: "OfferGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferLineItem_ProjectHourlyRateGroupId",
                table: "OfferLineItem",
                column: "ProjectHourlyRateGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OfferLineItem");

            migrationBuilder.DropTable(
                name: "OfferGroup");

            migrationBuilder.DropTable(
                name: "Offer");
        }
    }
}
