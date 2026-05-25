using FluentValidation;
using Financeiro.Business.Shared;

namespace Financeiro.Business.Aggregates.Validações
{
	public class EffectiveDateValidator : AbstractValidator<EffectiveDate>
	{
		public EffectiveDateValidator()
		{
			RuleFor(x => x.Year).GreaterThan(2000).WithMessage("O ano deve ser maior que 2000.");
			RuleFor(x => x.Month).IsInEnum().WithMessage("O mês informado é inválido.");
		}
	}
}
