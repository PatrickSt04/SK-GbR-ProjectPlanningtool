using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAAS_Projectplanningtool.Migrations
{
    /// <inheritdoc />
    public partial class AddHolidayCalendarEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (ActiveProvider.Contains("Sqlite", StringComparison.OrdinalIgnoreCase))
            {
                migrationBuilder.CreateTable(
                    name: "HolidayCalendarEntry",
                    columns: table => new
                    {
                        HolidayCalendarEntryId = table.Column<string>(type: "TEXT", nullable: false),
                        CompanyId = table.Column<string>(type: "TEXT", nullable: true),
                        HolidayName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                        HolidayDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                        HolidayType = table.Column<int>(type: "INTEGER", nullable: false),
                        LatestModifierId = table.Column<string>(type: "TEXT", nullable: true),
                        LatestModificationTimestamp = table.Column<DateTime>(type: "TEXT", nullable: true),
                        LatestModificationText = table.Column<string>(type: "TEXT", nullable: true),
                        CreatedById = table.Column<string>(type: "TEXT", nullable: true),
                        CreatedTimestamp = table.Column<DateTime>(type: "TEXT", nullable: true),
                        DeleteFlag = table.Column<bool>(type: "INTEGER", nullable: false)
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
            }
            else
            {
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
            }

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HolidayCalendarEntry");
        }
    }
}
