using System.ComponentModel.DataAnnotations;

namespace Financeiro.Application.Models.PayableModels
{
	public class CardExpenseItemTransferRequest
	{
		[Required]
		public List<Guid> CardExpenseItemIds { get; set; }

		[Required]
		public Guid TargetEffectiveDateId { get; set; }
	}
}
