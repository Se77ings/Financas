using Financeiro.Application.Models.EntryModels;
using Financeiro.Business.Shared.ResultPattern;

namespace Financas.Application.Services.CategoryServices
{
    public interface ICategoryManagementService
    {
        Task<Result<List<EntriesByDescriptionView>>> GetUncategorizedEntriesAsync();
        Task<Result<List<EntriesByDescriptionView>>> GetEntriesByCategoryAsync(Guid categoryId);
        Task<Result<bool>> RenameEntriesAsync(RenameEntriesRequest request);
        Task<Result<List<CategorizedDescriptionView>>> GetCategorizedDescriptionsAsync();
        Task<Result<List<EntryView>>> SearchEntriesAsync(string? term);
    }
}
