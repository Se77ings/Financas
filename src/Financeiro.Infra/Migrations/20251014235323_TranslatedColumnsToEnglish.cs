using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financeiro.Infra.Migrations
{
    /// <inheritdoc />
    public partial class TranslatedColumnsToEnglish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContasAReceber");

            migrationBuilder.DropTable(
                name: "DespesaAVista");

            migrationBuilder.DropTable(
                name: "DespesaCartao_Cartao");

            migrationBuilder.DropTable(
                name: "DespesaCartao");

            migrationBuilder.DropTable(
                name: "ContasAPagar");

            migrationBuilder.DropTable(
                name: "Competencias");

            migrationBuilder.CreateTable(
                name: "EffectiveDate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Month = table.Column<string>(type: "varchar(100)", maxLength: 10, nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EffectiveDate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Payable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "varchar(100)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Observation = table.Column<string>(type: "varchar(100)", maxLength: 150, nullable: true),
                    EffectiveDateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payable_EffectiveDate_EffectiveDateId",
                        column: x => x.EffectiveDateId,
                        principalTable: "EffectiveDate",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Receivable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "varchar(100)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Discounts = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    EffectiveDateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receivable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Receivable_EffectiveDate_EffectiveDateId",
                        column: x => x.EffectiveDateId,
                        principalTable: "EffectiveDate",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CardExpenseMapping",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardExpenseMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardExpenseMapping_Payable_Id",
                        column: x => x.Id,
                        principalTable: "Payable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImmediateExpense",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImmediateExpense", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImmediateExpense_Payable_Id",
                        column: x => x.Id,
                        principalTable: "Payable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CardExpense_Expense",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentInstallment = table.Column<int>(type: "int", nullable: false),
                    QuantityInstallments = table.Column<int>(type: "int", nullable: false),
                    CardExpenseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardExpense_Expense", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardExpense_Expense_CardExpenseMapping_CardExpenseId",
                        column: x => x.CardExpenseId,
                        principalTable: "CardExpenseMapping",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CardExpense_Expense_Payable_Id",
                        column: x => x.Id,
                        principalTable: "Payable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardExpense_Expense_CardExpenseId",
                table: "CardExpense_Expense",
                column: "CardExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_Payable_EffectiveDateId",
                table: "Payable",
                column: "EffectiveDateId");

            migrationBuilder.CreateIndex(
                name: "IX_Receivable_EffectiveDateId",
                table: "Receivable",
                column: "EffectiveDateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardExpense_Expense");

            migrationBuilder.DropTable(
                name: "ImmediateExpense");

            migrationBuilder.DropTable(
                name: "Receivable");

            migrationBuilder.DropTable(
                name: "CardExpenseMapping");

            migrationBuilder.DropTable(
                name: "Payable");

            migrationBuilder.DropTable(
                name: "EffectiveDate");

            migrationBuilder.CreateTable(
                name: "Competencias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ano = table.Column<int>(type: "int", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Mes = table.Column<string>(type: "varchar(100)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Competencias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContasAPagar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompetenciaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataVencimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descontos = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Descricao = table.Column<string>(type: "varchar(100)", nullable: false),
                    Observacao = table.Column<string>(type: "varchar(100)", maxLength: 150, nullable: true),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "ContasAReceber",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompetenciaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descontos = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Descricao = table.Column<string>(type: "varchar(100)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContasAReceber", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContasAReceber_Competencias_CompetenciaId",
                        column: x => x.CompetenciaId,
                        principalTable: "Competencias",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DespesaAVista",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DespesaAVista", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DespesaAVista_ContasAPagar_Id",
                        column: x => x.Id,
                        principalTable: "ContasAPagar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DespesaCartao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DespesaCartao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DespesaCartao_ContasAPagar_Id",
                        column: x => x.Id,
                        principalTable: "ContasAPagar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DespesaCartao_Cartao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DespesaCartaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParcelaAtual = table.Column<int>(type: "int", nullable: false),
                    QtdParcelas = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DespesaCartao_Cartao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DespesaCartao_Cartao_ContasAPagar_Id",
                        column: x => x.Id,
                        principalTable: "ContasAPagar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DespesaCartao_Cartao_DespesaCartao_DespesaCartaoId",
                        column: x => x.DespesaCartaoId,
                        principalTable: "DespesaCartao",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContasAPagar_CompetenciaId",
                table: "ContasAPagar",
                column: "CompetenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_ContasAReceber_CompetenciaId",
                table: "ContasAReceber",
                column: "CompetenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_DespesaCartao_Cartao_DespesaCartaoId",
                table: "DespesaCartao_Cartao",
                column: "DespesaCartaoId");
        }
    }
}
