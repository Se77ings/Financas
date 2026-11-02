namespace Financeiro.Application.Models.DashboardModels
{
    /// <summary>Somatório de todas as competências.</summary>
    public class DashboardTotalsView
    {
        public decimal Income { get; set; }
        public decimal Expenses { get; set; }
        public decimal Balance { get; set; }
    }
}
