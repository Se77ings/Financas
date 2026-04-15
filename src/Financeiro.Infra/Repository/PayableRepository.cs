using Financeiro.Business.Aggregates.Interfaces;
using Financeiro.Business.Aggregates.Payable;
using Financeiro.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace Financeiro.Infra.Repository
{
	public class PayableRepository : IPayableRepository
	{
		private readonly FinanceiroDBContext _context;
		public PayableRepository(FinanceiroDBContext context)
		{
			_context = context;
		}

		public async Task AddAsync(ImmediateExpense entity)
		{
			_context.Payable.Add(entity);
			await SaveChangesAsync();

		}
		public async Task UpdateAsync(ImmediateExpense entity)
		{
			_context.Payable.Update(entity);
			await SaveChangesAsync();
		}
		public async Task RemoveAsync(ImmediateExpense entity)
		{
			_context.Payable.Remove(entity);
			await SaveChangesAsync();
		}
		public async Task<ImmediateExpense> GetByIdAsync(Guid id)
		{
			return await _context.Payable.OfType<ImmediateExpense>().FirstOrDefaultAsync(x=> x.Id == id);
		}

		public async Task<List<ImmediateExpense>> GetListAsync()
		{
			return await _context.Payable.OfType<ImmediateExpense>().ToListAsync();

		}
		public async Task<int> SaveChangesAsync()
		{
			return await _context.SaveChangesAsync();
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
			return await _context.Payable.AnyAsync(x => x.Description == description && x.DueDate == date && x.Id != id);
		}
	}
}
