using Financeiro.Business.Shared;

namespace Financeiro.Application.Models.EntryModels
{
    /// <summary>Lançamento de qualquer tipo: gasto, item de cartão ou recebimento.</summary>
    public class EntryView
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public decimal Total { get; set; }
        public DateOnly Date { get; set; }
        public CalendarMonth Month { get; set; }
        public int Year { get; set; }
        public string? CreditCardName { get; set; }
        public int? CurrentInstallment { get; set; }
        public int? QuantityInstallments { get; set; }
    }
}
