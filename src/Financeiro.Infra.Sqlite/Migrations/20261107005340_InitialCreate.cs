using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financeiro.Infra.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "NOCASE"),
                    Description = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "NOCASE"),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CreditCards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "NOCASE"),
                    ClosingDay = table.Column<int>(type: "INTEGER", nullable: false),
                    DueDay = table.Column<int>(type: "INTEGER", nullable: false),
                    Limit = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EffectiveDate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Month = table.Column<int>(type: "INTEGER", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EffectiveDate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecurrentPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "varchar(100)", maxLength: 150, nullable: false, collation: "NOCASE"),
                    Value = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    FrequencyType = table.Column<int>(type: "INTEGER", nullable: false),
                    CreditCardId = table.Column<Guid>(type: "TEXT", nullable: true),
                    StartDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CategoryId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurrentPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecurrentPayments_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RecurrentPayments_CreditCards_CreditCardId",
                        column: x => x.CreditCardId,
                        principalTable: "CreditCards",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Payable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "varchar(100)", nullable: false, collation: "NOCASE"),
                    Total = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    DueDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    SubTotal = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Discount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Observation = table.Column<string>(type: "varchar(100)", maxLength: 500, nullable: true, collation: "NOCASE"),
                    EffectiveDateId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CategoryId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payable_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Payable_EffectiveDate_EffectiveDateId",
                        column: x => x.EffectiveDateId,
                        principalTable: "EffectiveDate",
                        principalColumn: "Id",
                        // Ajuste à mão, fora do modelo: excluir competência com fatura vazia depende deste cascade
                        // (EffectiveDateRepository.RemoveAsync), como na migration CascadeDeleteOnFKs do SQL Server.
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Receivable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "varchar(100)", nullable: false, collation: "NOCASE"),
                    SubTotal = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Discounts = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Total = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    CategoryId = table.Column<Guid>(type: "TEXT", nullable: true),
                    EffectiveDateId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receivable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Receivable_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Receivable_EffectiveDate_EffectiveDateId",
                        column: x => x.EffectiveDateId,
                        principalTable: "EffectiveDate",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CardExpense",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreditCardId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardExpense", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardExpense_CreditCards_CreditCardId",
                        column: x => x.CreditCardId,
                        principalTable: "CreditCards",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CardExpense_Payable_Id",
                        column: x => x.Id,
                        principalTable: "Payable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImmediateExpense",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false)
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
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CurrentInstallment = table.Column<int>(type: "INTEGER", nullable: false),
                    QuantityInstallments = table.Column<int>(type: "INTEGER", nullable: false),
                    CardExpenseId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardExpense_Expense", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardExpense_Expense_CardExpense_CardExpenseId",
                        column: x => x.CardExpenseId,
                        principalTable: "CardExpense",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CardExpense_Expense_Payable_Id",
                        column: x => x.Id,
                        principalTable: "Payable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardExpense_CreditCardId",
                table: "CardExpense",
                column: "CreditCardId");

            migrationBuilder.CreateIndex(
                name: "IX_CardExpense_Expense_CardExpenseId",
                table: "CardExpense_Expense",
                column: "CardExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_EffectiveDate_Month_Year",
                table: "EffectiveDate",
                columns: new[] { "Month", "Year" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payable_CategoryId",
                table: "Payable",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Payable_EffectiveDateId",
                table: "Payable",
                column: "EffectiveDateId");

            migrationBuilder.CreateIndex(
                name: "IX_Receivable_CategoryId",
                table: "Receivable",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Receivable_EffectiveDateId",
                table: "Receivable",
                column: "EffectiveDateId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurrentPayments_CategoryId",
                table: "RecurrentPayments",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurrentPayments_CreditCardId",
                table: "RecurrentPayments",
                column: "CreditCardId");
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
                name: "RecurrentPayments");

            migrationBuilder.DropTable(
                name: "CardExpense");

            migrationBuilder.DropTable(
                name: "CreditCards");

            migrationBuilder.DropTable(
                name: "Payable");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "EffectiveDate");
        }
    }
}
