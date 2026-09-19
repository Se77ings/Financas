using Financeiro.Business.Aggregates.Receivable;
using Financeiro.Infra.Context;
using Microsoft.EntityFrameworkCore;
using Financeiro.Business.Interfaces;

namespace Financeiro.Infra.Repository
{
	public class ReceivableRepository : IReceivableRepository
	{
		private readonly FinanceiroDBContext _context;

		public ReceivableRepository(FinanceiroDBContext context)
		{
			_context = context;
		}
		public async Task AddAsync(Receivable entity)
		{
			_context.Receivable.Add(entity);
			await SaveChangesAsync();
		}

		public async Task UpdateAsync(Receivable entity)
		{
			_context.Receivable.Update(entity);
			await SaveChangesAsync();
		}

		public async Task RemoveAsync(Receivable entity)
		{
			_context.Receivable.Remove(entity);
			await SaveChangesAsync();
		}

		public async Task<Receivable> GetByIdAsync(Guid id)
		{
			return await _context.Receivable.FindAsync(id);
		}

		public async Task<List<Receivable>> GetListAsync()
		{
			return await _context.Receivable.ToListAsync();
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
			return await _context.Receivable.AnyAsync(x => x.Description == description && x.Date == date);
		}

		public async Task<bool> ExistsOtherAsync(Guid id, string description, DateOnly date)
		{
			return await _context.Receivable.AnyAsync(x => x.Description == description && x.Date == date && id != id);

		}
	}

}
