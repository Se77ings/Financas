namespace Financeiro.Application.Models.DashboardModels
{
    public class CategorySpendingView
    {
        /// <summary>Nulo agrupa os lançamentos sem categoria.</summary>
        public Guid? CategoryId { get; set; }
        public string Name { get; set; }
        public decimal Total { get; set; }
        public decimal Percentage { get; set; }
        public List<SpendingEntryView> Entries { get; set; } = new();
    }
}
