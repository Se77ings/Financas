namespace Financeiro.Application.Models.DashboardModels
{
    /// <summary>
    /// Sem competência selecionada (todos os anos), os quadros de um único mês vêm nulos e os históricos
    /// trazem uma linha por ano.
    /// </summary>
    public class DashboardView
    {
        public DashboardTotalsView Totals { get; set; }
        public DailySpendingView? DailySpending { get; set; }
        public SpendingPaceView? SpendingPace { get; set; }
        public List<CategorySpendingView> SpendingByCategory { get; set; } = new();
        public List<CreditCardInvoicePeriodView> CreditCardHistory { get; set; } = new();
        public FutureCommitmentsView? FutureCommitments { get; set; }
        public List<HistoryPeriodView> History { get; set; } = new();
    }
}
