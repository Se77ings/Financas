using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financeiro.Infra.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDeleteOnFKs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payable_EffectiveDate_EffectiveDateId",
                table: "Payable");

            migrationBuilder.AddForeignKey(
                name: "FK_Payable_EffectiveDate_EffectiveDateId",
                table: "Payable",
                column: "EffectiveDateId",
                principalTable: "EffectiveDate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payable_EffectiveDate_EffectiveDateId",
                table: "Payable");

            migrationBuilder.AddForeignKey(
                name: "FK_Payable_EffectiveDate_EffectiveDateId",
                table: "Payable",
                column: "EffectiveDateId",
                principalTable: "EffectiveDate",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
