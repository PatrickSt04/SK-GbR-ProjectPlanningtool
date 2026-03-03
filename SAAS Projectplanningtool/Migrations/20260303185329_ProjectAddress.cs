using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAAS_Projectplanningtool.Migrations
{
    /// <inheritdoc />
    public partial class ProjectAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Project",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Project",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Project",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Project",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "Project");
        }
    }
}
