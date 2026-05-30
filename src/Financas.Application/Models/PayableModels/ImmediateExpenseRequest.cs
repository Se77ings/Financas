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

		/// <summary>
		/// Opcional. Se omitido, a competência é resolvida automaticamente pelo mês/ano de <see cref="DueDate"/>.
		/// </summary>
		public Guid? EffectiveDateId { get; set; }
	}
}
