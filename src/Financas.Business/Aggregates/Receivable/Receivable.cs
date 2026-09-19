using Financeiro.Business.Shared;

namespace Financeiro.Business.Aggregates.Receivable
{
	public class Receivable : Entity
	{
		public string Description { get; private set; }
		public decimal SubTotal { get; private set; }
		public decimal Discounts { get; private set; } = 0;
		public decimal Total { get; private set; }
		public DateOnly Date { get; private set; }

		/// EF Relation
		public Guid EffectiveDateId { get; private set; }
		public EffectiveDate EffectiveDate { get; private set; }

		protected Receivable() { }


		public Receivable(string description, decimal subtotal, DateOnly date, decimal? discounts, Guid effectiveDateId)
		{
			Description = description;
			SubTotal = subtotal;
			Discounts = discounts.HasValue ? discounts.Value : 0;
			Date = date;
			Total = SubTotal - Discounts;
			EffectiveDateId = effectiveDateId;
		}

		public void Update(string description, decimal subtotal, DateOnly date, decimal? discounts, Guid effectiveDateId)
		{
			Description = description;
			SubTotal = subtotal;
			Discounts = discounts.HasValue ? discounts.Value : 0;
			Date = date;
			Total = SubTotal - Discounts;
			EffectiveDateId = effectiveDateId;
		}
	}

}
