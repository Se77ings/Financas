namespace Financeiro.Application.Models.DashboardModels
{
    public class DailySpendingView
    {
        public List<DailySpendingDayView> Days { get; set; } = new();
    }
}
