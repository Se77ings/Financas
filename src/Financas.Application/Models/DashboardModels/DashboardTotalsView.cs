namespace Financeiro.Application.Models.DashboardModels
{
    /// <summary>Somatório das competências do ano selecionado, ou de todas.</summary>
    public class DashboardTotalsView
    {
        public decimal Income { get; set; }
        public decimal Expenses { get; set; }
        public decimal Balance { get; set; }
    }
}
