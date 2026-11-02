namespace Financeiro.Application.Models.DashboardModels
{
    public class SpendingPaceView
    {
        public decimal IdealDaily { get; set; }
        public List<SpendingPaceDayView> Days { get; set; } = new();
    }
}
