using Financeiro.Business.Aggregates.Payable;

namespace Financeiro.Application.Models.CreditCardModels
{
	public class CreditCardView
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public int ClosingDay { get; set; }
		public int DueDay { get; set; }
		public decimal Limit { get; set; }
		public bool IsActive { get; set; }

		public static implicit operator CreditCardView(CreditCard card)
		{
			if (card == null) return null;
			
			return new CreditCardView
			{
				Id = card.Id,
				Name = card.Name,
				ClosingDay = card.ClosingDay,
				DueDay = card.DueDay,
				Limit = card.Limit,
				IsActive = card.IsActive
			};
		}
	}
}
