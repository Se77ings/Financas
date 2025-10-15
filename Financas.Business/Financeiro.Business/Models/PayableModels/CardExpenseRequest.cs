using System.ComponentModel.DataAnnotations;

namespace Financeiro.Business.Models.PayableModels
{
	public class CardExpenseRequest
	{
		[Required]
		public string Description { get; set; }
		[Required]
		public DateOnly DueDate { get; set; }
		[Required]
		public decimal SubTotal { get; set; }
		[Required]
		public decimal Discounts { get; set; } = 0;
		public string? Observation { get; set; }
		[Required]
		public Guid EffectiveDate { get; set; }
	}

	public class CardExpenseItemRequest
	{
		[Required]
		public string Description { get; set; }
		[Required]
		public DateOnly DueDate { get; set; }
		[Required]
		public decimal SubTotal { get; set; }
		[Required]
		public decimal Discounts { get; set; } = 0;
		public string? Observation { get; set; }
		public int CurrentInstallment { get; set; } = 1;
		public int QuantityInstallments { get; set; } = 1;
	}
}
