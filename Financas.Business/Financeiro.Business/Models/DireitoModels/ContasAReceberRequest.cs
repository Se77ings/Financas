using System.ComponentModel.DataAnnotations;

namespace Financeiro.Business.Models.DireitoModels
{
	public class ContasAReceberRequest
	{
		[Required]
		public string Descricao { get; set; }
		[Required]
		public decimal SubTotal { get; set; }
		[Required]
		public decimal Descontos { get; set; }
		[Required]
		public DateTime Data { get; set; }
		[Required]
		public Guid CompetenciaId { get; set; }

	}
}
