using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financeiro.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Receivable",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Payable",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Receivable_CategoryId",
                table: "Receivable",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Payable_CategoryId",
                table: "Payable",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payable_Categories_CategoryId",
                table: "Payable",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Receivable_Categories_CategoryId",
                table: "Receivable",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payable_Categories_CategoryId",
                table: "Payable");

            migrationBuilder.DropForeignKey(
                name: "FK_Receivable_Categories_CategoryId",
                table: "Receivable");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Receivable_CategoryId",
                table: "Receivable");

            migrationBuilder.DropIndex(
                name: "IX_Payable_CategoryId",
                table: "Payable");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Receivable");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Payable");
        }
    }
}
