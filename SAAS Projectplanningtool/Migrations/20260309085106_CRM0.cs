using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAAS_Projectplanningtool.Migrations
{
    /// <inheritdoc />
    public partial class CRM0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactHistoryEntry_Company_CompanyId",
                table: "ContactHistoryEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerContactPerson_Company_CompanyId",
                table: "CustomerContactPerson");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerMaterialSurcharge_Company_CompanyId",
                table: "CustomerMaterialSurcharge");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerMessage_Company_CompanyId",
                table: "CustomerMessage");

            migrationBuilder.DropIndex(
                name: "IX_CustomerMessage_CompanyId",
                table: "CustomerMessage");

            migrationBuilder.DropIndex(
                name: "IX_CustomerMaterialSurcharge_CompanyId",
                table: "CustomerMaterialSurcharge");

            migrationBuilder.DropIndex(
                name: "IX_CustomerContactPerson_CompanyId",
                table: "CustomerContactPerson");

            migrationBuilder.DropIndex(
                name: "IX_ContactHistoryEntry_CompanyId",
                table: "ContactHistoryEntry");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "CustomerMessage");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "CustomerMaterialSurcharge");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "CustomerContactPerson");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "ContactHistoryEntry");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyId",
                table: "CustomerMessage",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CompanyId",
                table: "CustomerMaterialSurcharge",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CompanyId",
                table: "CustomerContactPerson",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CompanyId",
                table: "ContactHistoryEntry",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerMessage_CompanyId",
                table: "CustomerMessage",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerMaterialSurcharge_CompanyId",
                table: "CustomerMaterialSurcharge",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerContactPerson_CompanyId",
                table: "CustomerContactPerson",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactHistoryEntry_CompanyId",
                table: "ContactHistoryEntry",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactHistoryEntry_Company_CompanyId",
                table: "ContactHistoryEntry",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerContactPerson_Company_CompanyId",
                table: "CustomerContactPerson",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerMaterialSurcharge_Company_CompanyId",
                table: "CustomerMaterialSurcharge",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerMessage_Company_CompanyId",
                table: "CustomerMessage",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "CompanyId");
        }
    }
}
