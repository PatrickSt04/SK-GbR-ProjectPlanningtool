using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAAS_Projectplanningtool.Migrations
{
    /// <inheritdoc />
    public partial class projbudg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsedBudget",
                table: "ProjectBudget");

            migrationBuilder.AlterColumn<double>(
                name: "InitialBudget",
                table: "ProjectBudget",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "InitialBudget",
                table: "ProjectBudget",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<float>(
                name: "UsedBudget",
                table: "ProjectBudget",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
