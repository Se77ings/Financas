using FluentValidation;

namespace Financeiro.Business.Aggregates.Validações
{
	public class CompetenciaValidator : AbstractValidator<Competencia>
	{
		public CompetenciaValidator()
		{
			RuleFor(x => x.Ano).GreaterThan(2000).WithMessage("O ano deve ser maior que 2000.");
			RuleFor(x => x.Mes).MaximumLength(9).WithMessage("O mês deve ser escrito por extenso, e pode ter no máximo 9 caracteres");
		}
	}
}
