using Financeiro.Business.Aggregates.Payable;
using Financeiro.Business.Aggregates.Receivable;
using Financeiro.Business.Interfaces;
using Financeiro.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace Financeiro.Infra.Repository
{
    public class CategoryEntriesRepository : ICategoryEntriesRepository
    {
        private readonly FinanceiroDBContext _context;

        public CategoryEntriesRepository(FinanceiroDBContext context)
        {
            _context = context;
        }

        public async Task<List<ImmediateExpense>> GetImmediateExpensesByCategoryAsync(Guid? categoryId)
        {
            return await _context.Set<ImmediateExpense>()
                .Include(x => x.EffectiveDate)
                .Where(x => x.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<List<CardExpense.Expense>> GetCardExpenseItemsByCategoryAsync(Guid? categoryId)
        {
            return await _context.CardExpenseItem
                .Include(x => x.EffectiveDate)
                .Include(x => x.CardExpense).ThenInclude(x => x.CreditCard)
                .Where(x => x.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<List<Receivable>> GetReceivablesByCategoryAsync(Guid? categoryId)
        {
            return await _context.Receivable
                .Include(x => x.EffectiveDate)
                .Where(x => x.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<int> CountExistingPayablesAsync(IReadOnlyCollection<Guid> payableIds)
        {
            return await _context.Payable.CountAsync(x => payableIds.Contains(x.Id));
        }

        public async Task<int> CountExistingReceivablesAsync(IReadOnlyCollection<Guid> receivableIds)
        {
            return await _context.Receivable.CountAsync(x => receivableIds.Contains(x.Id));
        }

        public async Task SetPayablesDescriptionAsync(IReadOnlyCollection<Guid> payableIds, string description)
        {
            await _context.Payable
                .Where(x => payableIds.Contains(x.Id))
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.Description, description));
        }

        public async Task SetReceivablesDescriptionAsync(IReadOnlyCollection<Guid> receivableIds, string description)
        {
            await _context.Receivable
                .Where(x => receivableIds.Contains(x.Id))
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.Description, description));
        }
    }
}
