using Financeiro.Business.Shared;

namespace Financeiro.Application.Models.DashboardModels
{
    public class CreditCardInvoiceMonthView
    {
        public Guid EffectiveDateId { get; set; }
        public CalendarMonth Month { get; set; }
        public int Year { get; set; }
        public bool IsSelected { get; set; }

        /// <summary>Só os cartões com fatura na competência.</summary>
        public List<CreditCardInvoiceAmountView> Invoices { get; set; } = new();
    }
}
