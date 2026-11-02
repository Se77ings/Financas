using Financeiro.Business.Aggregates.Category;

namespace Financeiro.Business.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<bool> ExistsAsync(string name);
        Task<bool> ExistsOtherAsync(Guid id, string name);
        Task<int> CountLinkedEntriesAsync(Guid id);
        Task<Dictionary<Guid, int>> CountLinkedEntriesByCategoryAsync();

        Task<int> CountExistingPayablesAsync(IReadOnlyCollection<Guid> payableIds);
        Task<int> CountExistingReceivablesAsync(IReadOnlyCollection<Guid> receivableIds);
        Task SetPayablesCategoryAsync(IReadOnlyCollection<Guid> payableIds, Guid categoryId);
        Task SetReceivablesCategoryAsync(IReadOnlyCollection<Guid> receivableIds, Guid categoryId);
    }
}
