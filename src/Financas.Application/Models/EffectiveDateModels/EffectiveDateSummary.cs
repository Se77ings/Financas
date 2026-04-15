namespace Financeiro.Application.Models.EffectiveDateModels
{
	public class EffectiveDateSummary
	{
		public Guid Id { get; set; }
		public string Month { get; set; }
		public int Year { get; set; }

        // Melhorias de exibição
        public string Display => $"{Month}/{Year}";

        public int ReceivableCount { get; set; }
        public int PayableCount { get; set; }

        public decimal Balance { get; set; }

	}
}
