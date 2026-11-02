namespace Financeiro.Application.Models.DashboardModels
{
    public class DailySpendingView
    {
        public List<DailySpendingDayView> LastDays { get; set; } = new();

        /// <summary>Todos os dias do mês da competência selecionada, pela data de cada lançamento.</summary>
        public List<DailySpendingDayView> SelectedMonth { get; set; } = new();
    }
}
