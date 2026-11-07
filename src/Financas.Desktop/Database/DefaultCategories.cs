using Financas.Application.Services.CategoryServices;
using Financeiro.Application.Models.CategoryModels;
using Microsoft.Extensions.Logging;

namespace Financas.Desktop.Database
{
    public static class DefaultCategories
    {
        public static readonly IReadOnlyList<string> Names =
        [
            "Moradia",
            "Alimentação",
            "Transporte",
            "Saúde",
            "Educação",
            "Lazer",
            "Assinaturas",
            "Vestuário",
            "Impostos e taxas",
            "Investimentos",
            "Salário",
            "Outros"
        ];

        public static async Task CreateAsync(ICategoryService categories, ILogger logger)
        {
            foreach (var name in Names)
            {
                var result = await categories.AddAsync(new CategoryRequest { Name = name });

                if (!result.IsSuccess)
                    logger.LogWarning("Categoria inicial {Name} não criada: {Error}", name, result.Error!.Message);
            }
        }
    }
}
