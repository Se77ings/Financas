using System;
using Financeiro.Business.Shared;

namespace Financeiro.Business.Aggregates.Payable
{
	public class RecurrentPayment : Entity
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

		protected RecurrentPayment() { }

		public RecurrentPayment(string description, decimal value, FrequencyType frequencyType, Guid? creditCardId, DateOnly startDate, DateOnly? endDate, Guid? categoryId, bool isActive = true)
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

		/// <summary>
		/// Indica se a assinatura gera lançamento na competência informada, pela vigência e pela frequência
		/// contada a partir do mês de início. Não considera <see cref="IsActive"/>.
		/// </summary>
		public bool IsDueIn(CalendarMonth month, int year)
		{
			var competenceStart = new DateOnly(year, (int)month, 1);
			var competenceEnd = new DateOnly(year, (int)month, DateTime.DaysInMonth(year, (int)month));

			if (StartDate > competenceEnd)
				return false;

			if (EndDate.HasValue && EndDate.Value < competenceStart)
				return false;

			var diffMonths = (year - StartDate.Year) * 12 + ((int)month - StartDate.Month);
			if (diffMonths < 0)
				return false;

			return FrequencyType switch
			{
				FrequencyType.Mensal => true,
				FrequencyType.Trimestral => diffMonths % 3 == 0,
				FrequencyType.Semestral => diffMonths % 6 == 0,
				FrequencyType.Anual => diffMonths % 12 == 0,
				_ => false
			};
		}
	}
}
