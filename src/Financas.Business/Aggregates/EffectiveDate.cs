using System.Text;

namespace Financeiro.Business.Aggregates
{
	public class EffectiveDate : Entity
	{
		public string Month { get; private set; }
		public int Year { get; private set; }
		private List<Receivable.Receivable> _receivable = new();
		private List<Payable.Payable> _payable = new();

		public IEnumerable<Receivable.Receivable> Receivable => _receivable;
		public IEnumerable<Payable.Payable> Payable => _payable;

		protected EffectiveDate() { }
		public EffectiveDate(string month, int year)
		{
			Month = month;
			Year = year;
		}

		public void Update(string month, int year)
		{
			Month = month;
			Year = year;
		}

		public void AddPayable(Payable.Payable payable)
		{
			_payable.Add(payable);
		}

		public void AddReceivable(Receivable.Receivable receivable)
		{
			_receivable.Add(receivable);
		}

		public decimal CalculateTotal()
		{
			var pay = _payable.Sum(a => a.Total);
			var receive = _receivable.Sum(a => a.Total);

			return receive - pay;
		}

		public void GenerateFinancialStatement()
		{
			throw new NotImplementedException();
		}

		//public override string ToString()
		//{
		//	var sb = new StringBuilder();

		//	sb.AppendLine("===================");
		//	sb.AppendLine($"Competência: {Month} de {Year}");
		//	sb.AppendLine("===================");
		//	sb.AppendLine("Contas a Receber:");

		//	if (_receivable.Any())
		//	{
		//		foreach (var receber in _receivable)
		//		{
		//			sb.AppendLine($" - {receber.Description}: R$ {receber.Total:F2} (Vencimento: {receber.Date.ToShortDateString()})");
		//		}
		//	}
		//	else
		//	{
		//		sb.AppendLine(" Nenhuma conta a receber.");
		//	}

		//	sb.AppendLine("===================");
		//	sb.AppendLine("Contas a Pagar:");

		//	if (_payable.Any())
		//	{
		//		foreach (var pagar in _payable)
		//		{
		//			sb.AppendLine($" - {pagar.Description}: R$ {pagar.Total:F2} (Vencimento: {pagar.DueDate.ToShortDateString()})");
		//		}
		//	}
		//	else
		//	{
		//		sb.AppendLine(" Nenhuma conta a pagar.");
		//	}

		//	sb.AppendLine("===================");
		//	sb.AppendLine("Saldo: " + CalculateTotal());
		//	return sb.ToString();
		//}

	}
}
