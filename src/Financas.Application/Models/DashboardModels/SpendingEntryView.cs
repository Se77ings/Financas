namespace Financeiro.Application.Models.DashboardModels
{
    public class SpendingEntryView
    {
        public string Description { get; set; }
        public DateOnly Date { get; set; }
        public decimal Total { get; set; }

        /// <summary>Nulo quando o lançamento não tem categoria.</summary>
        public Guid? CategoryId { get; set; }

        /// <summary>Nulo para gastos à vista.</summary>
        public string? CreditCardName { get; set; }

        /// <summary>"2/3" para itens parcelados; nulo nos demais.</summary>
        public string? Installment { get; set; }
    }
}
