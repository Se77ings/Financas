using Financeiro.Business.Shared;

namespace Financeiro.Application.Models.DashboardModels
{
    /// <summary>Uma competência, ou um ano inteiro quando <see cref="Month"/> e <see cref="EffectiveDateId"/> vêm nulos.</summary>
    public class CreditCardInvoicePeriodView
    {
        public Guid? EffectiveDateId { get; set; }
        public CalendarMonth? Month { get; set; }
        public int Year { get; set; }
        public bool IsSelected { get; set; }

        /// <summary>Só os cartões com fatura no período.</summary>
        public List<CreditCardInvoiceAmountView> Invoices { get; set; } = new();
    }
}
