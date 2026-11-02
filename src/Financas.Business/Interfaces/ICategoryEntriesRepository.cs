using Financeiro.Business.Aggregates.Payable;
using Financeiro.Business.Aggregates.Receivable;

namespace Financeiro.Business.Interfaces
{
    public interface ICategoryEntriesRepository
    {
        Task<List<ImmediateExpense>> GetImmediateExpensesByCategoryAsync(Guid? categoryId);
        Task<List<CardExpense.Expense>> GetCardExpenseItemsByCategoryAsync(Guid? categoryId);
        Task<List<Receivable>> GetReceivablesByCategoryAsync(Guid? categoryId);

        Task<int> CountExistingPayablesAsync(IReadOnlyCollection<Guid> payableIds);
        Task<int> CountExistingReceivablesAsync(IReadOnlyCollection<Guid> receivableIds);
        Task SetPayablesDescriptionAsync(IReadOnlyCollection<Guid> payableIds, string description);
        Task SetReceivablesDescriptionAsync(IReadOnlyCollection<Guid> receivableIds, string description);
    }
}
