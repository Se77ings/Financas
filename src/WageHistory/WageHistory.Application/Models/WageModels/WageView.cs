using WageHistory.Business.Aggregates;

namespace WageHistory.Application.Models.WageModels
{
	public class WageView
	{
		public Guid Id { get; set; }
		public decimal GrossValue { get; set; }
		public decimal NetValue { get; set; }
		public DateOnly PaymentDate { get; set; }
		public List<DeductionView> Deductions { get; set; } = new List<DeductionView>();

		public static implicit operator WageView(Wage wage)
		{
			return new WageView
			{
				Id = wage.Id,
				GrossValue = wage.GrossValue,
				NetValue = wage.NetValue,
				PaymentDate = wage.PaymentDate,
				Deductions = wage.Deductions.Select(d => new DeductionView
				{
					Description = d.Description,
					Value = d.Value
				}).ToList()
			};
		}
	}
}
