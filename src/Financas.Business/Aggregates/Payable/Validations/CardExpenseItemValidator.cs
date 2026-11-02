using FluentValidation;

namespace Financeiro.Business.Aggregates.Payable.Validations
{
	public class CardExpenseItemValidator : AbstractValidator<CardExpense.Expense>
	{
		public CardExpenseItemValidator()
		{
			RuleFor(x => x.Description).NotEmpty().WithMessage("A descrição deve ser informada.");
			RuleFor(x => x.Description).MaximumLength(100).WithMessage("A descrição deve ter no máximo 100 caracteres.");
			RuleFor(x => x.Discount).GreaterThanOrEqualTo(0).WithMessage("Os descontos não podem ser negativos.");
			RuleFor(x => x.DueDate).NotEmpty().WithMessage("A data de vencimento não pode ser vazia.");
			RuleFor(x => x.CurrentInstallment).GreaterThanOrEqualTo(1).WithMessage("A parcela atual deve ser no mínimo 1.");
			RuleFor(x => x.QuantityInstallments).GreaterThanOrEqualTo(1).WithMessage("A quantidade de parcelas deve ser no mínimo 1.");
			RuleFor(x => x.CurrentInstallment).LessThanOrEqualTo(x => x.QuantityInstallments).WithMessage("A parcela atual não pode ser maior que a quantidade de parcelas.");
		}
	}
}
