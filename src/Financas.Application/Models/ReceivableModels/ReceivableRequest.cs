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

		/// <summary>
		/// Opcional. Se omitido, a competência é resolvida automaticamente pelo mês/ano de <see cref="Date"/>.
		/// </summary>
		public Guid? EffectiveDateId { get; set; }
        public Guid? CategoryId { get; set; }

    }
}
