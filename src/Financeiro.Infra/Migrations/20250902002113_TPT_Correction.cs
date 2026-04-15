using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financeiro.Infra.Migrations
{
    /// <inheritdoc />
    public partial class TPT_Correction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.DropColumn(
                name: "CompetenciaId",
                table: "DespesaCartao_Cartao");

            migrationBuilder.DropColumn(
                name: "CriadoEm",
                table: "DespesaCartao_Cartao");

            migrationBuilder.DropColumn(
                name: "DataVencimento",
                table: "DespesaCartao_Cartao");

            migrationBuilder.DropColumn(
                name: "Descontos",
                table: "DespesaCartao_Cartao");

            migrationBuilder.DropColumn(
                name: "Descricao",
                table: "DespesaCartao_Cartao");

            migrationBuilder.DropColumn(
                name: "Observacao",
                table: "DespesaCartao_Cartao");

            migrationBuilder.DropColumn(
                name: "SubTotal",
                table: "DespesaCartao_Cartao");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "DespesaCartao_Cartao");

            migrationBuilder.DropColumn(
                name: "CompetenciaId",
                table: "DespesaCartao");

            migrationBuilder.DropColumn(
                name: "CriadoEm",
                table: "DespesaCartao");

            migrationBuilder.DropColumn(
                name: "DataVencimento",
                table: "DespesaCartao");

            migrationBuilder.DropColumn(
                name: "Descontos",
                table: "DespesaCartao");

            migrationBuilder.DropColumn(
                name: "Descricao",
                table: "DespesaCartao");

            migrationBuilder.DropColumn(
                name: "Observacao",
                table: "DespesaCartao");

            migrationBuilder.DropColumn(
                name: "SubTotal",
                table: "DespesaCartao");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "DespesaCartao");

            migrationBuilder.DropColumn(
                name: "CompetenciaId",
                table: "DespesaAVista");

            migrationBuilder.DropColumn(
                name: "CriadoEm",
                table: "DespesaAVista");

            migrationBuilder.DropColumn(
                name: "DataVencimento",
                table: "DespesaAVista");

            migrationBuilder.DropColumn(
                name: "Descontos",
                table: "DespesaAVista");

            migrationBuilder.DropColumn(
                name: "Descricao",
                table: "DespesaAVista");

            migrationBuilder.DropColumn(
                name: "Observacao",
                table: "DespesaAVista");

            migrationBuilder.DropColumn(
                name: "SubTotal",
                table: "DespesaAVista");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "DespesaAVista");

            migrationBuilder.CreateTable(
                name: "ContasAPagar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Descricao = table.Column<string>(type: "varchar(100)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataVencimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Descontos = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Observacao = table.Column<string>(type: "varchar(100)", maxLength: 150, nullable: true),
                    CompetenciaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContasAPagar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContasAPagar_Competencias_CompetenciaId",
                        column: x => x.CompetenciaId,
                        principalTable: "Competencias",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContasAPagar_CompetenciaId",
                table: "ContasAPagar",
                column: "CompetenciaId");

            migrationBuilder.AddForeignKey(
                name: "FK_DespesaAVista_ContasAPagar_Id",
                table: "DespesaAVista",
                column: "Id",
                principalTable: "ContasAPagar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DespesaCartao_ContasAPagar_Id",
                table: "DespesaCartao",
                column: "Id",
                principalTable: "ContasAPagar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DespesaCartao_Cartao_ContasAPagar_Id",
                table: "DespesaCartao_Cartao",
                column: "Id",
                principalTable: "ContasAPagar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DespesaAVista_ContasAPagar_Id",
                table: "DespesaAVista");

            migrationBuilder.DropForeignKey(
                name: "FK_DespesaCartao_ContasAPagar_Id",
                table: "DespesaCartao");

            migrationBuilder.DropForeignKey(
                name: "FK_DespesaCartao_Cartao_ContasAPagar_Id",
                table: "DespesaCartao_Cartao");

            migrationBuilder.DropTable(
                name: "ContasAPagar");

            migrationBuilder.AddColumn<Guid>(
                name: "CompetenciaId",
                table: "DespesaCartao_Cartao",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CriadoEm",
                table: "DespesaCartao_Cartao",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataVencimento",
                table: "DespesaCartao_Cartao",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "Descontos",
                table: "DespesaCartao_Cartao",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Descricao",
                table: "DespesaCartao_Cartao",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Observacao",
                table: "DespesaCartao_Cartao",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotal",
                table: "DespesaCartao_Cartao",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "DespesaCartao_Cartao",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "CompetenciaId",
                table: "DespesaCartao",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CriadoEm",
                table: "DespesaCartao",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataVencimento",
                table: "DespesaCartao",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "Descontos",
                table: "DespesaCartao",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Descricao",
                table: "DespesaCartao",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Observacao",
                table: "DespesaCartao",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotal",
                table: "DespesaCartao",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "DespesaCartao",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "CompetenciaId",
                table: "DespesaAVista",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CriadoEm",
                table: "DespesaAVista",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataVencimento",
                table: "DespesaAVista",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "Descontos",
                table: "DespesaAVista",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Descricao",
                table: "DespesaAVista",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Observacao",
                table: "DespesaAVista",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotal",
                table: "DespesaAVista",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "DespesaAVista",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

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
    }
}
