using Financeiro.Business.Aggregates.Payable;
using FluentValidation;

namespace Financeiro.Business.Aggregates.Validações
{
	public class ImmediateExpenseValidator : AbstractValidator<ImmediateExpense>
	{
		public ImmediateExpenseValidator()
		{
			RuleFor(x=> x.Description).MaximumLength(100).WithMessage("A descrição deve ter no máximo 100 caracteres.");
			RuleFor(x => x.Discount).LessThanOrEqualTo(x => x.SubTotal).WithMessage("Os descontos não podem ser maiores que o subtotal.");
			RuleFor(x => x.SubTotal).GreaterThan(0).WithMessage("O subtotal deve ser maior que zero.");
			RuleFor(x => x.DueDate).NotEmpty().WithMessage("A data de vencimento não pode ser vazia.");
		}
	}
}
