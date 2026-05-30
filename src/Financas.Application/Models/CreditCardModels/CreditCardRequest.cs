using System.ComponentModel.DataAnnotations;

namespace Financeiro.Application.Models.CreditCardModels
{
	public class CreditCardRequest
	{
		[Required(ErrorMessage = "O nome do cartão é obrigatório.")]
		[MaxLength(100, ErrorMessage = "O nome do cartão deve ter no máximo 100 caracteres.")]
		public string Name { get; set; }

		[Required(ErrorMessage = "O dia de fechamento é obrigatório.")]
		[Range(1, 31, ErrorMessage = "O dia de fechamento deve ser entre 1 e 31.")]
		public int ClosingDay { get; set; }

		[Required(ErrorMessage = "O dia de vencimento é obrigatório.")]
		[Range(1, 31, ErrorMessage = "O dia de vencimento deve ser entre 1 e 31.")]
		public int DueDay { get; set; }

		[Required(ErrorMessage = "O limite é obrigatório.")]
		[Range(0, double.MaxValue, ErrorMessage = "O limite não pode ser negativo.")]
		public decimal Limit { get; set; }

		public bool IsActive { get; set; } = true;
	}
}
