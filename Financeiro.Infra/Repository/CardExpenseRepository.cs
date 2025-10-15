using Financeiro.Business.Aggregates.Interfaces;
using Financeiro.Business.Aggregates.Payable;
using Financeiro.Infra.Context;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

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
			return await _context.Payable.OfType<CardExpense>().Include(x => x.Expenses).FirstOrDefaultAsync(x => x.Id == id);
		}

		public async Task<List<CardExpense>> GetListAsync()
		{
			return await _context.Payable.OfType<CardExpense>().ToListAsync();
		}
		public async Task<int> SaveChangesAsync()
		{
			return await _context.SaveChangesAsync();
		}

		public async Task AddItemAsync(CardExpense.Expense item)
		{
			_context.CardExpenseItem.Add(item);
		}

		public void Dispose()
		{
			_context?.Dispose();
		}

		public async Task<bool> ExistsAsync(string description, DateOnly date)
		{
			return await _context.Payable.AnyAsync(x => x.Description == description && x.DueDate == date);
		}

		public async Task<bool> ExistsOtherAsync(Guid id, string description, DateOnly date)
		{
			return await _context.Payable.AnyAsync(x => x.Description == description && x.DueDate == date && x.Id == id);       
		}

		public async Task<bool> ExistsItemAsync(string description, DateOnly date)
		{
			return await _context.CardExpenseItem.AnyAsync(x => x.Description == description && x.DueDate == date);
				}

		public async Task<bool> ExistsOtherItemAsync(Guid id, string description, DateOnly date)
		{
			return await _context.CardExpenseItem.AnyAsync(x => x.Description == description && x.DueDate == date && x.Id == id);
		}
	}
}
