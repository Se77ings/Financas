using System.ComponentModel.DataAnnotations;

namespace Financeiro.Business.Models.EffectiveDateModels
{
	public class EffectiveDateRequest
	{
		[Required]
		public string Month { get; set; }
		[Required]
		public int Year { get; set; }

	}
}
