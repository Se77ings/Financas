using Financeiro.Business.Aggregates;
using Financeiro.Business.Aggregates.Payable;
using Financeiro.Business.Models.ReceivableModels;
using Financeiro.Business.Models.PayableModels;

namespace Financeiro.Business.Models.EffectiveDateModels
{
	public class EffectiveDateView
	{

		public string Month { get; set; }
		public int Year { get; set; }

		public List<ReceivableView> Receivable { get; set; } = new();
		public List<ImmediateExpenseView> Payable { get; set; } = new();
		public List<CardExpenseView> CardExpense { get; set; } = new();



		public static implicit operator EffectiveDateView(EffectiveDate effectiveDate)
		{
			return new EffectiveDateView
			{
				Month = effectiveDate.Month,
				Year = effectiveDate.Year,
				Receivable = effectiveDate.Receivable.Select(c => (ReceivableView)c).ToList(),
				Payable = effectiveDate.Payable.OfType<ImmediateExpense>().Select(d => (ImmediateExpenseView)d).ToList(),
				CardExpense = effectiveDate.Payable.OfType<CardExpense>().Select(d => (CardExpenseView)d).ToList()
			};
		}

	}
}
