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

        #region Subscription Info

        [RequiredIf(nameof(isSubscription), ErrorMessage = "O tipo de frequência é obrigatório.")]
        public FrequencyType SubscriptionFrequencyType { get; set; }

        [RequiredIf(nameof(isSubscription), ErrorMessage = "A data de início é obrigatória.")]
        public DateOnly SubscriptionStartDate { get; set; }

        public DateOnly? SubscriptionEndDate { get; set; }

        public bool isSubscription { get; set; } = false;
        #endregion
    }
}
