using FluentValidation;
using CategoryEntity = Financeiro.Business.Aggregates.Category.Category;

namespace Financeiro.Business.Aggregates.CategoryValidators
{
    public class CategoryValidator : AbstractValidator<CategoryEntity>
    {
        public CategoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("O nome da categoria não pode ser vazio.")
                .MaximumLength(100)
                .WithMessage("O nome da categoria deve ter no máximo 100 caracteres.");

            RuleFor(x => x.Description)
                .MaximumLength(100)
                .WithMessage("A descrição da categoria deve ter no máximo 100 caracteres.");
        }
    }
}
