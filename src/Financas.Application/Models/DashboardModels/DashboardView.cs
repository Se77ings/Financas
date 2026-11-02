namespace Financeiro.Application.Models.DashboardModels
{
    public class DashboardView
    {
        public DashboardTotalsView Totals { get; set; }
        public DailySpendingView DailySpending { get; set; }
        public SpendingPaceView SpendingPace { get; set; }
        public List<CategorySpendingView> SpendingByCategory { get; set; } = new();
        public List<CreditCardUsageView> CreditCards { get; set; } = new();
        public FutureCommitmentsView FutureCommitments { get; set; }
        public List<MonthlyHistoryView> MonthlyHistory { get; set; } = new();
    }
}
