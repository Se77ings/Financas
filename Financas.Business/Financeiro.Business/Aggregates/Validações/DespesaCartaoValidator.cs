using Financeiro.Business.Aggregates.Obrigação;
using FluentValidation;

namespace Financeiro.Business.Aggregates.Validações
{
	public class DespesaCartaoValidator : AbstractValidator<DespesaCartao>
	{
		public DespesaCartaoValidator()
		{
			RuleFor(x => x.Descricao).MaximumLength(100).WithMessage("A descrição deve ter no máximo 100 caracteres.");
			RuleFor(x => x.Descontos).LessThanOrEqualTo(x => x.SubTotal).WithMessage("Os descontos não podem ser maiores que o subtotal.");
			RuleFor(x => x.SubTotal).GreaterThanOrEqualTo(0).WithMessage("O subtotal não pode ser negativo.");
			RuleFor(x => x.DataVencimento).NotEmpty().WithMessage("A data de vencimento não pode ser vazia.");
		}
	}
}
