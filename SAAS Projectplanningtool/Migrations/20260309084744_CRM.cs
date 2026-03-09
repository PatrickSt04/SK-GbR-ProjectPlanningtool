using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAAS_Projectplanningtool.Migrations
{
    /// <inheritdoc />
    public partial class CRM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "Customer",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                table: "Customer",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerType",
                table: "Customer",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Customer",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Customer",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ContactHistoryEntry",
                columns: table => new
                {
                    EntryId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContactType = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactHistoryEntry", x => x.EntryId);
                    table.ForeignKey(
                        name: "FK_ContactHistoryEntry_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_ContactHistoryEntry_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "CustomerId");
                    table.ForeignKey(
                        name: "FK_ContactHistoryEntry_Employee_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                });

            migrationBuilder.CreateTable(
                name: "CustomerContactPerson",
                columns: table => new
                {
                    ContactPersonId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                        name: "FK_CustomerContactPerson_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
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
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaterialCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SurchargePercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerMaterialSurcharge", x => x.SurchargeId);
                    table.ForeignKey(
                        name: "FK_CustomerMaterialSurcharge_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
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
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                        name: "FK_CustomerMessage_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_CustomerMessage_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "CustomerId");
                    table.ForeignKey(
                        name: "FK_CustomerMessage_Employee_ClosedById",
                        column: x => x.ClosedById,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_CustomerMessage_Employee_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContactHistoryEntry_CompanyId",
                table: "ContactHistoryEntry",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactHistoryEntry_CreatedById",
                table: "ContactHistoryEntry",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ContactHistoryEntry_CustomerId",
                table: "ContactHistoryEntry",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerContactPerson_CompanyId",
                table: "CustomerContactPerson",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerContactPerson_CustomerId",
                table: "CustomerContactPerson",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerMaterialSurcharge_CompanyId",
                table: "CustomerMaterialSurcharge",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerMaterialSurcharge_CustomerId",
                table: "CustomerMaterialSurcharge",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerMessage_ClosedById",
                table: "CustomerMessage",
                column: "ClosedById");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerMessage_CompanyId",
                table: "CustomerMessage",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerMessage_CreatedById",
                table: "CustomerMessage",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerMessage_CustomerId",
                table: "CustomerMessage",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactHistoryEntry");

            migrationBuilder.DropTable(
                name: "CustomerContactPerson");

            migrationBuilder.DropTable(
                name: "CustomerMaterialSurcharge");

            migrationBuilder.DropTable(
                name: "CustomerMessage");

            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "CustomerType",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Customer");
        }
    }
}
