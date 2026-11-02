using FluentValidation;

namespace Financeiro.Business.Aggregates.Receivable.Validations
{
	public class ReceivableValidator : AbstractValidator<Receivable>
	{
		public ReceivableValidator()
		{
			RuleFor(x => x.Description).MaximumLength(100).WithMessage("A descrição deve ter no máximo 100 caracteres.");
			RuleFor(x => x.Discounts).LessThanOrEqualTo(x => x.SubTotal).WithMessage("Os descontos não podem ser maiores que o subtotal.");
			RuleFor(x => x.SubTotal).GreaterThan(0).WithMessage("O subtotal deve ser maior que zero.");
			RuleFor(x => x.Date).NotEmpty().WithMessage("A data não pode ser vazia.");
		}
	}
}
