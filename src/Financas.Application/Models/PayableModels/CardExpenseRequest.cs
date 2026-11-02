using Financas.Domain.Validations;
using Financeiro.Business.Shared;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Financeiro.Application.Models.PayableModels
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

        public Guid CreditCardId { get; set; }
        public Guid? CategoryId { get; set; }

        /// <summary>
        /// Opcional. Se omitido, a competência é resolvida automaticamente pelo mês/ano de <see cref="DueDate"/>.
        /// </summary>
        public Guid? EffectiveDateId { get; set; }

        [JsonPropertyName("effectiveDate")]
        public Guid? EffectiveDate
        {
            get => EffectiveDateId;
            set => EffectiveDateId = value;
        }
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
