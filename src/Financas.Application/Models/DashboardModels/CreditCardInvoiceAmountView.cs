namespace Financeiro.Application.Models.DashboardModels
{
    public class CreditCardInvoiceAmountView
    {
        public Guid CreditCardId { get; set; }
        public string Name { get; set; }
        public decimal Total { get; set; }
    }
}
