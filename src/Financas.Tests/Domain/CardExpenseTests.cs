using Financeiro.Business.Aggregates.Payable;
using Financeiro.Business.Aggregates.Payable.Validations;

namespace Financas.Tests.Domain
{
    public class CardExpenseTests
    {
        private static readonly DateOnly DueDate = new(2026, 11, 10);

        private static CardExpense NewInvoice() =>
            new("Fatura", 0, DueDate, 0, null!, Guid.NewGuid(), Guid.NewGuid(), null);

        private static CardExpense.Expense NewItem(CardExpense invoice, decimal subtotal, decimal discount = 0, int current = 1, int quantity = 1) =>
            new("Item", subtotal, DueDate, discount, null!, current, quantity, invoice.EffectiveDateId, invoice.Id, null);

        [Fact]
        public void AddExpense_RecalculaOsTotaisDaFatura()
        {
            var invoice = NewInvoice();

            invoice.AddExpense(NewItem(invoice, 100, discount: 10));
            invoice.AddExpense(NewItem(invoice, 50));

            Assert.Equal(150, invoice.SubTotal);
            Assert.Equal(10, invoice.Discount);
            Assert.Equal(140, invoice.Total);
        }

        [Fact]
        public void RemoveExpense_RecalculaOsTotaisDaFatura()
        {
            var invoice = NewInvoice();
            var removed = NewItem(invoice, 100);
            invoice.AddExpense(removed);
            invoice.AddExpense(NewItem(invoice, 50));

            invoice.RemoveExpense(removed.Id);

            Assert.Equal(50, invoice.Total);
        }

        [Fact]
        public void UpdateExpense_RecalculaOsTotaisDaFatura()
        {
            var invoice = NewInvoice();
            var item = NewItem(invoice, 100);
            invoice.AddExpense(item);

            invoice.UpdateExpense(item.Id, "Item", 80, DueDate, 5, null!, 1, 1, null);

            Assert.Equal(75, invoice.Total);
        }

        [Theory]
        [InlineData(1, 1, true)]
        [InlineData(3, 10, true)]
        [InlineData(10, 10, true)]
        [InlineData(11, 10, false)]
        [InlineData(0, 1, false)]
        public void Validator_ExigeParcelaAtualEntreUmEAQuantidade(int current, int quantity, bool expectedValid)
        {
            var invoice = NewInvoice();
            var item = NewItem(invoice, 100, current: current, quantity: quantity);

            var result = new CardExpenseItemValidator().Validate(item);

            Assert.Equal(expectedValid, result.IsValid);
        }
    }
}
