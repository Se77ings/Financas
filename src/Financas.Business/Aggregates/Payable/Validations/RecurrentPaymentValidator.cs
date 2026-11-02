using Financeiro.Business.Aggregates.Payable;
using FluentValidation;

namespace Financeiro.Business.Aggregates.Payable.Validations
{
	public class RecurrentPaymentValidator : AbstractValidator<RecurrentPayment>
	{
		public RecurrentPaymentValidator()
		{
			RuleFor(x => x.Description).NotEmpty().MaximumLength(150).WithMessage("A descrição deve ter no máximo 150 caracteres.");
			RuleFor(x => x.Value).GreaterThan(0).WithMessage("O valor da assinatura deve ser maior que zero.");
			RuleFor(x => x.FrequencyType).IsInEnum().WithMessage("Tipo de frequência inválido.");
			RuleFor(x => x.StartDate).NotEmpty().WithMessage("A data de início deve ser informada.");
			RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate).When(x => x.EndDate.HasValue).WithMessage("A data de término não pode ser anterior à data de início.");
		}
	}
}
