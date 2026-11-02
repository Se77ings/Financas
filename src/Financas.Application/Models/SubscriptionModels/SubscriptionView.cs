using System;
using Financeiro.Business.Aggregates.Payable;
using Financeiro.Business.Shared;

namespace Financeiro.Application.Models.SubscriptionModels
{
	public class SubscriptionView
	{
		public Guid Id { get; set; }
		public string Description { get; set; }
		public decimal Value { get; set; }
		public FrequencyType FrequencyType { get; set; }
		public string FrequencyName => FrequencyType.ToString();
		public Guid? CreditCardId { get; set; }
		public string? CreditCardName { get; set; }
		public DateOnly StartDate { get; set; }
		public DateOnly? EndDate { get; set; }
		public bool IsActive { get; set; }
		public Guid? CategoryId { get; set; }

		public static implicit operator SubscriptionView(Subscription sub)
		{
			if (sub == null) return null;

			return new SubscriptionView
			{
				Id = sub.Id,
				Description = sub.Description,
				Value = sub.Value,
				FrequencyType = sub.FrequencyType,
				CreditCardId = sub.CreditCardId,
				CreditCardName = sub.CreditCard?.Name,
				StartDate = sub.StartDate,
				EndDate = sub.EndDate,
				IsActive = sub.IsActive,
				CategoryId = sub.CategoryId
			};
		}
	}
}
