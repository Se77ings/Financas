using CategoryEntity = Financeiro.Business.Aggregates.Category.Category;

namespace Financeiro.Application.Models.CategoryModels
{
    public class CategoryView
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        /// <summary>
        /// Lançamentos (contas a pagar e a receber) vinculados. Preenchido apenas na listagem.
        /// </summary>
        public int EntriesCount { get; set; }

        public static implicit operator CategoryView(CategoryEntity category)
        {
            return new CategoryView
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }
    }
}
