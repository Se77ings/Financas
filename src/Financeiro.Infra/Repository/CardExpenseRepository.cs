using Financeiro.Business.Aggregates.Payable;
using Financeiro.Business.Interfaces;
using Financeiro.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace Financeiro.Infra.Repository
{
    public class CardExpenseRepository : ICardExpenseRepository
    {
        private readonly FinanceiroDBContext _context;
        public CardExpenseRepository(FinanceiroDBContext context)
        {
            _context = context;
        }
        public async Task AddAsync(CardExpense entity)
        {
            _context.Payable.Add(entity);
            await SaveChangesAsync();
        }


        public async Task UpdateAsync(CardExpense entity)
        {
            _context.Payable.Update(entity);
            await SaveChangesAsync();
        }

        public async Task RemoveAsync(CardExpense entity)
        {
            _context.Payable.Remove(entity);
            await SaveChangesAsync();

        }

        public async Task<CardExpense> GetByIdAsync(Guid id)
        {
            return await _context.Payable
                .AsSplitQuery()
                .AsNoTracking()
                .OfType<CardExpense>()
                .Include(x => x.Expenses.OrderByDescending(e => e.DueDate).ThenByDescending(e => e.CreatedAt))
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<CardExpense>> GetListAsync()
        {
            return await _context.Payable
                .AsSplitQuery()
                .AsNoTracking()
                .OfType<CardExpense>()
                .Include(x => x.Expenses)
                .ToListAsync();
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void AddItem(CardExpense invoice, CardExpense.Expense item)
        {
            _context.CardExpenseItem.Add(item);
            _context.Entry(invoice).State = EntityState.Modified;
        }

        public async Task RemoveItemAsync(CardExpense.Expense item)
        {
            _context.CardExpenseItem.Remove(item);
            await SaveChangesAsync();
        }


        public async Task<bool> ExistsAsync(string description, DateOnly date)
        {
            return await _context.Payable.AnyAsync(x => x.Description == description && x.DueDate == date);
        }

        public async Task<bool> ExistsOtherAsync(Guid id, string description, DateOnly date)
        {
            return await _context.Payable.AnyAsync(x => x.Description == description && x.DueDate == date && x.Id != id);
        }

        public async Task<bool> ExistsItemAsync(string description, DateOnly date)
        {
            return await _context.CardExpenseItem.AnyAsync(x => x.Description == description && x.DueDate == date);
        }

        public async Task<bool> ExistsOtherItemAsync(Guid id, string description, DateOnly date)
        {
            return await _context.CardExpenseItem.AnyAsync(x => x.Description == description && x.DueDate == date && x.Id != id);
        }

        public async Task<CardExpense?> GetByCardAndEffectiveDateAsync(Guid creditCardId, Guid effectiveDateId)
        {
            return await _context.Payable
                .AsSplitQuery()
                .AsNoTracking()
                .OfType<CardExpense>()
                .Include(x => x.Expenses)
                .FirstOrDefaultAsync(x => x.CreditCardId == creditCardId && x.EffectiveDateId == effectiveDateId);
        }

        public async Task<CardExpense?> GetLatestByCreditCardAsync(Guid creditCardId)
        {
            return await _context.Payable
                .AsSplitQuery()
                .AsNoTracking()
                .OfType<CardExpense>()
                .Include(x => x.Expenses)
                .Where(x => x.CreditCardId == creditCardId)
                .OrderByDescending(x => x.EffectiveDate.Year)
                .ThenByDescending(x => x.EffectiveDate.Month)
                .FirstOrDefaultAsync();
        }

        public async Task<List<CardExpense>> GetCardExpensesByItemIdsAsync(List<Guid> itemIds)
        {
            return await _context.Payable
                .AsSplitQuery()
                .AsNoTracking()
                .OfType<CardExpense>()
                .Include(x => x.Expenses)
                .Where(c => c.Expenses.Any(e => itemIds.Contains(e.Id)))
                .ToListAsync();
        }

        public async Task UpdateItemAsync(CardExpense.Expense item)
        {
            _context.CardExpenseItem.Update(item);
            await SaveChangesAsync();
        }
    }
}
