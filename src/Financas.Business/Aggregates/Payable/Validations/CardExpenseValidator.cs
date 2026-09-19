using Financeiro.Business.Aggregates.Payable;
using FluentValidation;

namespace Financeiro.Business.Aggregates.Payable.Validations
{
	public class CardExpenseValidator : AbstractValidator<CardExpense>
	{
		public CardExpenseValidator()
		{
			RuleFor(x => x.Description).MaximumLength(100).WithMessage("A descrição deve ter no máximo 100 caracteres.");
			RuleFor(x => x.Discount).LessThanOrEqualTo(x => x.SubTotal).WithMessage("Os descontos não podem ser maiores que o subtotal.");
			RuleFor(x => x.SubTotal).GreaterThanOrEqualTo(0).WithMessage("O subtotal não pode ser negativo.");
			RuleFor(x => x.DueDate).NotEmpty().WithMessage("A data de vencimento não pode ser vazia.");
		}
	}
}
