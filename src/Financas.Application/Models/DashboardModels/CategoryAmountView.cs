namespace Financeiro.Application.Models.DashboardModels
{
    public class CategoryAmountView
    {
        /// <summary>Nulo agrupa os lançamentos sem categoria.</summary>
        public Guid? CategoryId { get; set; }
        public decimal Total { get; set; }
    }
}
