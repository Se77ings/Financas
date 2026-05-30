using System;
using System.ComponentModel.DataAnnotations;
using Financeiro.Business.Shared;

namespace Financeiro.Application.Models.SubscriptionModels
{
	public class SubscriptionRequest
	{
		[Required(ErrorMessage = "A descrição é obrigatória.")]
		[MaxLength(150, ErrorMessage = "A descrição deve ter no máximo 150 caracteres.")]
		public string Description { get; set; }

		[Required(ErrorMessage = "O valor é obrigatório.")]
		[Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
		public decimal Value { get; set; }

		[Required(ErrorMessage = "O tipo de frequência é obrigatório.")]
		public FrequencyType FrequencyType { get; set; }

		public Guid? CreditCardId { get; set; }

		[Required(ErrorMessage = "A data de início é obrigatória.")]
		public DateOnly StartDate { get; set; }

		public DateOnly? EndDate { get; set; }

		public bool IsActive { get; set; } = true;
	}
}
