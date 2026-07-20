using System.ComponentModel.DataAnnotations;

namespace WageHistory.Application.Models.WageModels
{
	public class DeductionRequest
	{
		[Required]
		public string Description { get; set; } = string.Empty;

		[Required]
		public decimal Value { get; set; }
	}
}
