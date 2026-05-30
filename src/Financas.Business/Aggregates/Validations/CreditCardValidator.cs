using Financeiro.Business.Aggregates.Payable;
using FluentValidation;

namespace Financeiro.Business.Aggregates.Validações
{
	public class CreditCardValidator : AbstractValidator<CreditCard>
	{
		public CreditCardValidator()
		{
			RuleFor(x => x.Name).NotEmpty().MaximumLength(100).WithMessage("O nome do cartão deve ter no máximo 100 caracteres.");
			RuleFor(x => x.ClosingDay).InclusiveBetween(1, 31).WithMessage("O dia de fechamento deve ser entre 1 e 31.");
			RuleFor(x => x.DueDay).InclusiveBetween(1, 31).WithMessage("O dia de vencimento deve ser entre 1 e 31.");
			RuleFor(x => x.Limit).GreaterThanOrEqualTo(0).WithMessage("O limite não pode ser negativo.");
		}
	}
}
