using Financeiro.Business.Aggregates.Direito;
using Financeiro.Business.Aggregates.Obrigação;
using System.Text;

namespace Financeiro.Business.Aggregates
{
	public class Competencia : Entidade
	{
		public string Mes { get; private set; }
		public int Ano { get; private set; }
		private List<ContasAReceber> _contasAReceber = new();
		private List<ContasAPagar> _contasAPagar = new();

		public IEnumerable<ContasAReceber> ContasAReceber => _contasAReceber;
		public IEnumerable<ContasAPagar> ContasAPagar => _contasAPagar;

		protected Competencia() { }
		public Competencia(string mes, int ano)
		{
			Mes = mes;
			Ano = ano;
		}

		public void Atualizar(string mes, int ano)
		{
			//TODO: Validação, provavelmente por conta da classe de serviço.
			Mes = mes;
			Ano = ano;
		}

		public void AdicionarContasAPagar(ContasAPagar aPagar)
		{
			_contasAPagar.Add(aPagar);
		}

		public void AdicionarContasAReceber(ContasAReceber aReceber)
		{
			_contasAReceber.Add(aReceber);
		}

		public decimal CalcularTotal()
		{
			var pagar = _contasAPagar.Sum(a => a.Total);
			var receber = _contasAReceber.Sum(a => a.Total);

			return receber - pagar;
		}

		public void GerarExtrato()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			var sb = new StringBuilder();

			sb.AppendLine("===================");
			sb.AppendLine($"Competência: {Mes} de {Ano}");
			sb.AppendLine("===================");
			sb.AppendLine("Contas a Receber:");

			if (_contasAReceber.Any())
			{
				foreach (var receber in _contasAReceber)
				{
					sb.AppendLine($" - {receber.Descricao}: R$ {receber.Total:F2} (Vencimento: {receber.Data.ToShortDateString()})");
				}
			}
			else
			{
				sb.AppendLine(" Nenhuma conta a receber.");
			}

			sb.AppendLine("===================");
			sb.AppendLine("Contas a Pagar:");

			if (_contasAPagar.Any())
			{
				foreach (var pagar in _contasAPagar)
				{
					sb.AppendLine($" - {pagar.Descricao}: R$ {pagar.Total:F2} (Vencimento: {pagar.DataVencimento.ToShortDateString()})");
				}
			}
			else
			{
				sb.AppendLine(" Nenhuma conta a pagar.");
			}

			sb.AppendLine("===================");
			sb.AppendLine("Saldo: " + CalcularTotal());
			return sb.ToString();
		}

	}
}
