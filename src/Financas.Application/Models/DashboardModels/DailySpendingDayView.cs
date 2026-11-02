namespace Financeiro.Application.Models.DashboardModels
{
    public class DailySpendingDayView
    {
        public DateOnly Date { get; set; }
        public decimal Total { get; set; }
        public List<CategoryAmountView> ByCategory { get; set; } = new();

        /// <summary>Gasto ideal por dia da competência do mês desta data; nulo se a competência não existe.</summary>
        public decimal? IdealDaily { get; set; }
    }
}
