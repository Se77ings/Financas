namespace Financeiro.Business.Aggregates.Payable
{
	public abstract class Payable : Entity
	{
		public string Description { get; private set; }
		public decimal Total { get; private set; }
		public DateOnly DueDate { get; private set; }
		public decimal SubTotal { get; private set; }
		public decimal Discount { get; private set; } = 0;
		public string? Observation { get; private set; }
		public Guid EffectiveDateId { get; private set; }
		public EffectiveDate EffectiveDate { get; private set; }
		//TODO: Urgente: implementar bool recorrente.
		// caso for recorrente, ao gerar a proxima competencia, verificar na anterior se há dados recorrentes
		// caso haja, gerá-los apenas atualizando o mês


		protected Payable() { }

		public Payable(string description, decimal subtotal, DateOnly dueDate, decimal? discounts, string observation, Guid effectiveDateId)
		{
			Description = description;
			SubTotal = subtotal;
			DueDate = dueDate;
			Discount = discounts.HasValue ? discounts.Value : 0;
			Observation = observation;
			EffectiveDateId = effectiveDateId;

			CalculateTotal();
		}
		public void Update(string description, decimal subtotal, DateOnly dueDate, decimal? discounts, string observation, Guid effectiveDateId)
		{
			Description = description;
			SubTotal = subtotal;
			DueDate = dueDate;
			Discount = discounts.HasValue ? discounts.Value : 0;
			Observation = observation;
			Total = SubTotal - Discount;
			EffectiveDateId = effectiveDateId;

			CalculateTotal();
		}

		internal void SetValues(decimal subTotal, decimal discount)
		{
			this.SubTotal = subTotal;
			this.Discount = discount;
			CalculateTotal();
		}

		private void CalculateTotal()
		{
			this.Total = this.SubTotal - this.Discount;
		}
	}
}
