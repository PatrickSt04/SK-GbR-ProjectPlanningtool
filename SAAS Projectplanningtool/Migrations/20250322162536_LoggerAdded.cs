using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAAS_Projectplanningtool.Migrations
{
    /// <inheritdoc />
    public partial class LoggerAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_Logfile_ExcecutingEmployeeId",
                table: "Logfile",
                column: "ExcecutingEmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logfile");
        }
    }
}
