using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAAS_Projectplanningtool.Migrations
{
    /// <inheritdoc />
    public partial class timetracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TimeEntry");
        }
    }
}
