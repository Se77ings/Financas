using Financeiro.Business.Aggregates.Direito;
using FluentValidation;

namespace Financeiro.Business.Aggregates.Validações
{
	public class ContasAReceberValidador : AbstractValidator<ContasAReceber>
	{
		public ContasAReceberValidador()
		{
			RuleFor(x => x.Descricao).MaximumLength(100).WithMessage("A descrição deve ter no máximo 100 caracteres.");
			RuleFor(x => x.Descontos).LessThanOrEqualTo(x => x.SubTotal).WithMessage("Os descontos não podem ser maiores que o subtotal.");
			RuleFor(x => x.SubTotal).GreaterThan(0).WithMessage("O subtotal deve ser maior que zero.");
			RuleFor(x => x.Data).NotEmpty().WithMessage("A data não pode ser vazia.");
		}
	}
}
