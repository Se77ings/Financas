using System.ComponentModel.DataAnnotations;

namespace Financeiro.Business.Models.ObrigaçãoModels
{
	public class DespesaAVistaRequest
	{
		[Required]
		public string Descricao { get; set; }
		[Required]
		public DateTime DataVencimento { get; set; }
		[Required]
		public decimal SubTotal { get; set; }
		[Required]
		public decimal Descontos { get; set; }
		public string? Observacao { get; set; }
		[Required]
		public Guid CompetenciaId { get; set; }

	}
}
