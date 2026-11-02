using System.ComponentModel.DataAnnotations;
using Financas.Domain.Validations;
using Financeiro.Business.Shared;

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
        public Guid? CategoryId { get; set; }

        #region RecurrentPayment Info

        [RequiredIf(nameof(IsRecurrent), ErrorMessage = "O tipo de frequência é obrigatório.")]
        public FrequencyType RecurrenceFrequencyType { get; set; }

        [RequiredIf(nameof(IsRecurrent), ErrorMessage = "A data de início é obrigatória.")]
        public DateOnly RecurrenceStartDate { get; set; }

        public DateOnly? RecurrenceEndDate { get; set; }

        public bool IsRecurrent { get; set; } = false;
        #endregion
    }
}
