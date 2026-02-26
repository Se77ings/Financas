using System.ComponentModel.DataAnnotations;

namespace Financeiro.Application.Models.PayableModels
{
	public class ImmediateExpenseRequest
	{
		[Required]
		public string Description { get; set; }
		[Required]
		public DateOnly DueDate { get; set; }
		[Required]
		public decimal SubTotal { get; set; }
		[Required]
		public decimal Discounts { get; set; }
		public string? Observation { get; set; }
		[Required]
		public Guid EffectiveDateId { get; set; }

	}
}
