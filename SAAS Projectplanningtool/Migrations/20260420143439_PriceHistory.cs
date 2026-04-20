using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAAS_Projectplanningtool.Migrations
{
    /// <inheritdoc />
    public partial class PriceHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Article");

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
                    table.ForeignKey(
                        name: "FK_ArticlePriceHistory_Employee_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticlePriceHistory_ArticleId",
                table: "ArticlePriceHistory",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticlePriceHistory_CreatedById",
                table: "ArticlePriceHistory",
                column: "CreatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticlePriceHistory");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Article",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
