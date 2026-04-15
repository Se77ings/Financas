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

		public async Task RecalculateCardExpenseAsync(Guid cardExpenseId)
		{
			var cardExpense = await GetByIdAsync(cardExpenseId);
			if (cardExpense != null && cardExpense.Expenses.Any())
			{
				var subTotal = cardExpense.Expenses.Sum(e => e.SubTotal);
				var discounts = cardExpense.Expenses.Sum(e => e.Discount);
				

				await _context.Database.ExecuteSqlRawAsync(
					$"UPDATE Payable SET SubTotal = '{subTotal}', Discount = '{discounts}', Total = '{subTotal - discounts}' WHERE Id = '{cardExpenseId}'");
			}
		}

		public async Task RemoveItemAsync(CardExpense.Expense item)
		{
			_context.CardExpenseItem.Remove(item);
			await SaveChangesAsync();
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
