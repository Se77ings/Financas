using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financeiro.Infra.Migrations
{
    /// <inheritdoc />
    public partial class CreditCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CardExpense_Expense_CardExpenseMapping_CardExpenseId",
                table: "CardExpense_Expense");

            migrationBuilder.DropForeignKey(
                name: "FK_CardExpenseMapping_Payable_Id",
                table: "CardExpenseMapping");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CardExpenseMapping",
                table: "CardExpenseMapping");

            migrationBuilder.RenameTable(
                name: "CardExpenseMapping",
                newName: "CardExpense");

            migrationBuilder.AddColumn<Guid>(
                name: "CreditCardId",
                table: "CardExpense",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_CardExpense",
                table: "CardExpense",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CreditCards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    ClosingDay = table.Column<int>(type: "int", nullable: false),
                    DueDay = table.Column<int>(type: "int", nullable: false),
                    Limit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "varchar(100)", maxLength: 150, nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FrequencyType = table.Column<int>(type: "int", nullable: false),
                    CreditCardId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_CreditCards_CreditCardId",
                        column: x => x.CreditCardId,
                        principalTable: "CreditCards",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardExpense_CreditCardId",
                table: "CardExpense",
                column: "CreditCardId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_CreditCardId",
                table: "Subscriptions",
                column: "CreditCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_CardExpense_CreditCards_CreditCardId",
                table: "CardExpense",
                column: "CreditCardId",
                principalTable: "CreditCards",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CardExpense_Payable_Id",
                table: "CardExpense",
                column: "Id",
                principalTable: "Payable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CardExpense_Expense_CardExpense_CardExpenseId",
                table: "CardExpense_Expense",
                column: "CardExpenseId",
                principalTable: "CardExpense",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CardExpense_CreditCards_CreditCardId",
                table: "CardExpense");

            migrationBuilder.DropForeignKey(
                name: "FK_CardExpense_Payable_Id",
                table: "CardExpense");

            migrationBuilder.DropForeignKey(
                name: "FK_CardExpense_Expense_CardExpense_CardExpenseId",
                table: "CardExpense_Expense");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "CreditCards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CardExpense",
                table: "CardExpense");

            migrationBuilder.DropIndex(
                name: "IX_CardExpense_CreditCardId",
                table: "CardExpense");

            migrationBuilder.DropColumn(
                name: "CreditCardId",
                table: "CardExpense");

            migrationBuilder.RenameTable(
                name: "CardExpense",
                newName: "CardExpenseMapping");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CardExpenseMapping",
                table: "CardExpenseMapping",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CardExpense_Expense_CardExpenseMapping_CardExpenseId",
                table: "CardExpense_Expense",
                column: "CardExpenseId",
                principalTable: "CardExpenseMapping",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CardExpenseMapping_Payable_Id",
                table: "CardExpenseMapping",
                column: "Id",
                principalTable: "Payable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
