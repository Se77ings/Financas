using System.ComponentModel.DataAnnotations;
using Financeiro.Business.Shared;

namespace Financeiro.Application.Models.EffectiveDateModels
{
	public class EffectiveDateRequest
	{
		[Required]
		public CalendarMonth Month { get; set; }

		[Required]
		public int Year { get; set; }
	}
}
