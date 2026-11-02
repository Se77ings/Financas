using Financeiro.Application.Models.EntryModels;
using Financeiro.Business.Shared.ResultPattern;

namespace Financas.Application.Services.CategoryServices
{
    public interface ICategoryManagementService
    {
        Task<(Result<bool> result, List<EntriesByDescriptionView> data)> GetUncategorizedEntriesAsync();
        Task<(Result<bool> result, List<EntriesByDescriptionView> data)> GetEntriesByCategoryAsync(Guid categoryId);
        Task<Result<bool>> RenameEntriesAsync(RenameEntriesRequest request);
        Task<(Result<bool> result, List<CategorizedDescriptionView> data)> GetCategorizedDescriptionsAsync();
    }
}
