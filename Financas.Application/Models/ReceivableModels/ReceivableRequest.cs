using System.ComponentModel.DataAnnotations;

namespace Financeiro.Application.Models.ReceivableModels
{
	public class ReceivableRequest
	{
		[Required]
		public string Description { get; set; }
		[Required]
		public decimal SubTotal { get; set; }
		[Required]
		public decimal Discounts { get; set; }
		[Required]
		public DateOnly Date { get; set; }
		[Required]
		public Guid EffectiveDateId { get; set; }

	}
}
