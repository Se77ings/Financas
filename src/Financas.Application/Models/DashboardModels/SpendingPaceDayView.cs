namespace Financeiro.Application.Models.DashboardModels
{
    public class SpendingPaceDayView
    {
        public int Day { get; set; }

        /// <summary>Nulo para os dias ainda não transcorridos do mês corrente.</summary>
        public decimal? Accumulated { get; set; }
        public decimal? PreviousMonthAccumulated { get; set; }
        public decimal IdealAccumulated { get; set; }
    }
}
