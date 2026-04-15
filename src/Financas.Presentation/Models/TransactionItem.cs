using Financeiro.Application.Models.PayableModels;


namespace Financas.Presentation.Models
{
    public class TransactionItem
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Discounts { get; set; }
        public decimal Total => SubTotal - Discounts;
        public TransactionType Type { get; set; }
        public string TypeIcon => Type == TransactionType.Receivable ? "+" : "-";
        public string TypeColor => Type == TransactionType.Receivable ? "#4CAF50" : "#F44336";
        public bool IsCardExpense { get; set; }
        public List<CardExpenseItemView> CardItems { get; set; } = new();
    }

    public enum TransactionType
    {
        Receivable,
        ImmediateExpense,
        CardExpense
    }
}
