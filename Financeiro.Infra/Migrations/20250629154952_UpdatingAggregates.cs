using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financeiro.Infra.Migrations
{
    /// <inheritdoc />
    public partial class UpdatingAggregates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_contasAReceber_competencias_CompetenciaId",
                table: "contasAReceber");

            migrationBuilder.DropPrimaryKey(
                name: "PK_contasAReceber",
                table: "contasAReceber");

            migrationBuilder.DropPrimaryKey(
                name: "PK_competencias",
                table: "competencias");

            migrationBuilder.RenameTable(
                name: "contasAReceber",
                newName: "ContasAReceber");

            migrationBuilder.RenameTable(
                name: "competencias",
                newName: "Competencias");

            migrationBuilder.RenameIndex(
                name: "IX_contasAReceber_CompetenciaId",
                table: "ContasAReceber",
                newName: "IX_ContasAReceber_CompetenciaId");

            migrationBuilder.AlterColumn<Guid>(
                name: "DespesaCartaoId",
                table: "DespesaCartao_Cartao",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompetenciaId",
                table: "DespesaCartao_Cartao",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CompetenciaId",
                table: "DespesaCartao",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CompetenciaId",
                table: "DespesaAVista",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "CompetenciaId",
                table: "ContasAReceber",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContasAReceber",
                table: "ContasAReceber",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Competencias",
                table: "Competencias",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DespesaCartao_Cartao_CompetenciaId",
                table: "DespesaCartao_Cartao",
                column: "CompetenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_DespesaCartao_CompetenciaId",
                table: "DespesaCartao",
                column: "CompetenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_DespesaAVista_CompetenciaId",
                table: "DespesaAVista",
                column: "CompetenciaId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContasAReceber_Competencias_CompetenciaId",
                table: "ContasAReceber",
                column: "CompetenciaId",
                principalTable: "Competencias",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DespesaAVista_Competencias_CompetenciaId",
                table: "DespesaAVista",
                column: "CompetenciaId",
                principalTable: "Competencias",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DespesaCartao_Competencias_CompetenciaId",
                table: "DespesaCartao",
                column: "CompetenciaId",
                principalTable: "Competencias",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DespesaCartao_Cartao_Competencias_CompetenciaId",
                table: "DespesaCartao_Cartao",
                column: "CompetenciaId",
                principalTable: "Competencias",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContasAReceber_Competencias_CompetenciaId",
                table: "ContasAReceber");

            migrationBuilder.DropForeignKey(
                name: "FK_DespesaAVista_Competencias_CompetenciaId",
                table: "DespesaAVista");

            migrationBuilder.DropForeignKey(
                name: "FK_DespesaCartao_Competencias_CompetenciaId",
                table: "DespesaCartao");

            migrationBuilder.DropForeignKey(
                name: "FK_DespesaCartao_Cartao_Competencias_CompetenciaId",
                table: "DespesaCartao_Cartao");

            migrationBuilder.DropIndex(
                name: "IX_DespesaCartao_Cartao_CompetenciaId",
                table: "DespesaCartao_Cartao");

            migrationBuilder.DropIndex(
                name: "IX_DespesaCartao_CompetenciaId",
                table: "DespesaCartao");

            migrationBuilder.DropIndex(
                name: "IX_DespesaAVista_CompetenciaId",
                table: "DespesaAVista");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContasAReceber",
                table: "ContasAReceber");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Competencias",
                table: "Competencias");

            migrationBuilder.DropColumn(
                name: "CompetenciaId",
                table: "DespesaCartao_Cartao");

            migrationBuilder.DropColumn(
                name: "CompetenciaId",
                table: "DespesaCartao");

            migrationBuilder.DropColumn(
                name: "CompetenciaId",
                table: "DespesaAVista");

            migrationBuilder.RenameTable(
                name: "ContasAReceber",
                newName: "contasAReceber");

            migrationBuilder.RenameTable(
                name: "Competencias",
                newName: "competencias");

            migrationBuilder.RenameIndex(
                name: "IX_ContasAReceber_CompetenciaId",
                table: "contasAReceber",
                newName: "IX_contasAReceber_CompetenciaId");

            migrationBuilder.AlterColumn<Guid>(
                name: "DespesaCartaoId",
                table: "DespesaCartao_Cartao",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "CompetenciaId",
                table: "contasAReceber",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddPrimaryKey(
                name: "PK_contasAReceber",
                table: "contasAReceber",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_competencias",
                table: "competencias",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_contasAReceber_competencias_CompetenciaId",
                table: "contasAReceber",
                column: "CompetenciaId",
                principalTable: "competencias",
                principalColumn: "Id");
        }
    }
}
