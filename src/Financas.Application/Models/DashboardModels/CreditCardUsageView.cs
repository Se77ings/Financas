namespace Financeiro.Application.Models.DashboardModels
{
    public class CreditCardUsageView
    {
        public Guid CreditCardId { get; set; }
        public string Name { get; set; }
        public decimal Limit { get; set; }
        public int ClosingDay { get; set; }
        public int DueDay { get; set; }
        public decimal InvoiceTotal { get; set; }
        public DateOnly? InvoiceDueDate { get; set; }

        /// <summary>Parcelas restantes das compras parceladas presentes na fatura da competência.</summary>
        public decimal FutureInstallments { get; set; }
        public decimal Committed { get; set; }
        public decimal UsagePercentage { get; set; }
    }
}
