namespace WageHistory.Application.Models.WageModels
{
	public class WageSummary
	{
		public Guid Id { get; set; }
		public decimal GrossValue { get; set; }
		public decimal NetValue { get; set; }
		public DateOnly PaymentDate { get; set; }
	}
}
