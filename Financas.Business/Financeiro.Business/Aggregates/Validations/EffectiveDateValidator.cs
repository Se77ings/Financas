using FluentValidation;

namespace Financeiro.Business.Aggregates.Validações
{
	public class EffectiveDateValidator : AbstractValidator<EffectiveDate>
	{
		public EffectiveDateValidator()
		{
			RuleFor(x => x.Year).GreaterThan(2000).WithMessage("O ano deve ser maior que 2000.");
			RuleFor(x => x.Month).MaximumLength(9).WithMessage("O mês deve ser escrito por extenso, e pode ter no máximo 9 caracteres");
		}
	}
}
