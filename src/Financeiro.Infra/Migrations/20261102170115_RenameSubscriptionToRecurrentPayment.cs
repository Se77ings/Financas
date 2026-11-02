using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financeiro.Infra.Migrations
{
    /// <inheritdoc />
    // Escrita à mão: o EF gerou DropTable + CreateTable para a troca de nome da entidade, o que apagaria
    // as assinaturas existentes. Aqui a tabela, a chave, os índices e as FKs são apenas renomeados.
    public partial class RenameSubscriptionToRecurrentPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Subscriptions_Categories_CategoryId", table: "Subscriptions");
            migrationBuilder.DropForeignKey(name: "FK_Subscriptions_CreditCards_CreditCardId", table: "Subscriptions");
            migrationBuilder.DropPrimaryKey(name: "PK_Subscriptions", table: "Subscriptions");

            migrationBuilder.RenameTable(name: "Subscriptions", newName: "RecurrentPayments");

            migrationBuilder.RenameIndex(name: "IX_Subscriptions_CategoryId", table: "RecurrentPayments", newName: "IX_RecurrentPayments_CategoryId");
            migrationBuilder.RenameIndex(name: "IX_Subscriptions_CreditCardId", table: "RecurrentPayments", newName: "IX_RecurrentPayments_CreditCardId");

            migrationBuilder.AddPrimaryKey(name: "PK_RecurrentPayments", table: "RecurrentPayments", column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RecurrentPayments_Categories_CategoryId",
                table: "RecurrentPayments",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RecurrentPayments_CreditCards_CreditCardId",
                table: "RecurrentPayments",
                column: "CreditCardId",
                principalTable: "CreditCards",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_RecurrentPayments_Categories_CategoryId", table: "RecurrentPayments");
            migrationBuilder.DropForeignKey(name: "FK_RecurrentPayments_CreditCards_CreditCardId", table: "RecurrentPayments");
            migrationBuilder.DropPrimaryKey(name: "PK_RecurrentPayments", table: "RecurrentPayments");

            migrationBuilder.RenameTable(name: "RecurrentPayments", newName: "Subscriptions");

            migrationBuilder.RenameIndex(name: "IX_RecurrentPayments_CategoryId", table: "Subscriptions", newName: "IX_Subscriptions_CategoryId");
            migrationBuilder.RenameIndex(name: "IX_RecurrentPayments_CreditCardId", table: "Subscriptions", newName: "IX_Subscriptions_CreditCardId");

            migrationBuilder.AddPrimaryKey(name: "PK_Subscriptions", table: "Subscriptions", column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Categories_CategoryId",
                table: "Subscriptions",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_CreditCards_CreditCardId",
                table: "Subscriptions",
                column: "CreditCardId",
                principalTable: "CreditCards",
                principalColumn: "Id");
        }
    }
}
