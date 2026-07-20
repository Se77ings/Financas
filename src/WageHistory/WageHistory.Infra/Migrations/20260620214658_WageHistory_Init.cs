using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WageHistory.Infra.Migrations
{
    /// <inheritdoc />
    public partial class WageHistory_Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Wages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GrossValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WageDeductions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "varchar(100)", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WageDeductions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WageDeductions_Wages_WageId",
                        column: x => x.WageId,
                        principalTable: "Wages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_WageDeductions_WageId",
                table: "WageDeductions",
                column: "WageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WageDeductions");

            migrationBuilder.DropTable(
                name: "Wages");
        }
    }
}
