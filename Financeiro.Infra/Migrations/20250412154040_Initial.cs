using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financeiro.Infra.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "competencias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Mes = table.Column<string>(type: "varchar(100)", maxLength: 10, nullable: false),
                    Ano = table.Column<int>(type: "int", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_competencias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DespesaAVista",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataVencimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Descontos = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DespesaAVista", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DespesaCartao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataVencimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Descontos = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DespesaCartao", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "contasAReceber",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Descricao = table.Column<string>(type: "varchar(100)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Descontos = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompetenciaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contasAReceber", x => x.Id);
                    table.ForeignKey(
                        name: "FK_contasAReceber_competencias_CompetenciaId",
                        column: x => x.CompetenciaId,
                        principalTable: "competencias",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DespesaCartao_Cartao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParcelaAtual = table.Column<int>(type: "int", nullable: false),
                    QtdParcelas = table.Column<int>(type: "int", nullable: false),
                    DespesaCartaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataVencimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Descontos = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DespesaCartao_Cartao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DespesaCartao_Cartao_DespesaCartao_DespesaCartaoId",
                        column: x => x.DespesaCartaoId,
                        principalTable: "DespesaCartao",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_contasAReceber_CompetenciaId",
                table: "contasAReceber",
                column: "CompetenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_DespesaCartao_Cartao_DespesaCartaoId",
                table: "DespesaCartao_Cartao",
                column: "DespesaCartaoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contasAReceber");

            migrationBuilder.DropTable(
                name: "DespesaAVista");

            migrationBuilder.DropTable(
                name: "DespesaCartao_Cartao");

            migrationBuilder.DropTable(
                name: "competencias");

            migrationBuilder.DropTable(
                name: "DespesaCartao");
        }
    }
}
