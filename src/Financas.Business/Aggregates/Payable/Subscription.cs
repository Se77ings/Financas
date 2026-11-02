using System;
using Financeiro.Business.Shared;

namespace Financeiro.Business.Aggregates.Payable
{
	public class Subscription : Entity
	{
		public string Description { get; private set; }
		public decimal Value { get; private set; }
		public FrequencyType FrequencyType { get; private set; }
		public Guid? CreditCardId { get; private set; }
		public CreditCard? CreditCard { get; private set; }
		public DateOnly StartDate { get; private set; }
		public DateOnly? EndDate { get; private set; }
		public bool IsActive { get; private set; }
		public Guid? CategoryId { get; private set; }
		public Category.Category? Category { get; private set; }

		protected Subscription() { }

		public Subscription(string description, decimal value, FrequencyType frequencyType, Guid? creditCardId, DateOnly startDate, DateOnly? endDate, Guid? categoryId, bool isActive = true)
		{
			Description = description;
			Value = value;
			FrequencyType = frequencyType;
			CreditCardId = creditCardId;
			StartDate = startDate;
			EndDate = endDate;
			IsActive = isActive;
			CategoryId = categoryId;
		}

		public void Update(string description, decimal value, FrequencyType frequencyType, Guid? creditCardId, DateOnly startDate, DateOnly? endDate, Guid? categoryId, bool isActive)
		{
			Description = description;
			Value = value;
			FrequencyType = frequencyType;
			CreditCardId = creditCardId;
			StartDate = startDate;
			EndDate = endDate;
			IsActive = isActive;
			CategoryId = categoryId;
		}
	}
}
