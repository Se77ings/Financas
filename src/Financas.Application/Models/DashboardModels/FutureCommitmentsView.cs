namespace Financeiro.Application.Models.DashboardModels
{
    public class FutureCommitmentsView
    {
        /// <summary>Média dos recebimentos das últimas competências com receita, até a selecionada.</summary>
        public decimal AverageIncome { get; set; }
        public List<FutureCommitmentMonthView> Months { get; set; } = new();
    }
}
