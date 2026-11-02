using Financeiro.Application.Models.CategoryModels;
using Financeiro.Business.Shared.ResultPattern;

namespace Financas.Application.Services.CategoryServices
{
    public interface ICategoryService
    {
        Task<(Result<bool> result, CategoryView view)> GetByIdAsync(Guid id);
        Task<(Result<bool> result, List<CategoryView> data)> GetListAsync();
        Task<(Result<bool> result, CategoryView view)> AddAsync(CategoryRequest request);
        Task<Result<bool>> UpdateAsync(Guid id, CategoryRequest request);
        Task<Result<bool>> DeleteAsync(Guid id);
        Task<Result<bool>> LinkPayablesAsync(IEnumerable<Guid> payableIds, Guid categoryId);
        Task<Result<bool>> LinkReceivablesAsync(IEnumerable<Guid> receivableIds, Guid categoryId);
    }
}
