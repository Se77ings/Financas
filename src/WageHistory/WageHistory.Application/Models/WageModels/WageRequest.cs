using System.ComponentModel.DataAnnotations;

namespace WageHistory.Application.Models.WageModels
{
	public class WageRequest
	{
		[Required]
		public decimal GrossValue { get; set; }

		[Required]
		public decimal NetValue { get; set; }

		[Required]
		public DateOnly PaymentDate { get; set; }

		public List<DeductionRequest> Deductions { get; set; } = new List<DeductionRequest>();
	}
}
