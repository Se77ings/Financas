using Financeiro.Business.Aggregates.Payable;
using Financeiro.Business.Aggregates.Receivable;

namespace Financeiro.Business.Interfaces
{
    public interface ICategoryEntriesRepository
    {
        Task<List<ImmediateExpense>> GetImmediateExpensesByCategoryAsync(Guid? categoryId);
        Task<List<CardExpense.Expense>> GetCardExpenseItemsByCategoryAsync(Guid? categoryId);
        Task<List<Receivable>> GetReceivablesByCategoryAsync(Guid? categoryId);

        /// <summary>Lançamentos de qualquer categoria cuja descrição contém <paramref name="term"/>, ignorando maiúsculas e acentos.</summary>
        Task<(List<ImmediateExpense> ImmediateExpenses, List<CardExpense.Expense> CardExpenseItems, List<Receivable> Receivables)> SearchByDescriptionAsync(string term);

        /// <summary>Descrições dos lançamentos categorizados, com quantas vezes aparecem em cada categoria.</summary>
        Task<List<(string Description, Guid CategoryId, int Count)>> GetCategorizedDescriptionsAsync();

        Task<int> CountExistingPayablesAsync(IReadOnlyCollection<Guid> payableIds);
        Task<int> CountExistingReceivablesAsync(IReadOnlyCollection<Guid> receivableIds);
        Task SetPayablesDescriptionAsync(IReadOnlyCollection<Guid> payableIds, string description);
        Task SetReceivablesDescriptionAsync(IReadOnlyCollection<Guid> receivableIds, string description);
    }
}
