using Microsoft.EntityFrameworkCore;
using WageHistory.Business.Aggregates;
using WageHistory.Business.Aggregates.Interfaces;
using WageHistory.Infra.Context;

namespace WageHistory.Infra.Repository
{
	public class WageRepository : IWageRepository
	{
		private readonly WageHistoryDBContext _context;

		public WageRepository(WageHistoryDBContext context)
		{
			_context = context;
		}

		public async Task AddAsync(Wage entity)
		{
			_context.Wages.Add(entity);
			await SaveChangesAsync();
		}

		public async Task UpdateAsync(Wage entity)
		{
			var entry = _context.Entry(entity);
			if (entry.State == EntityState.Detached)
			{
				_context.Wages.Update(entity);
			}
			await SaveChangesAsync();
		}

		public async Task RemoveAsync(Wage entity)
		{
			_context.Wages.Remove(entity);
			await SaveChangesAsync();
		}

		public async Task<Wage?> GetByIdAsync(Guid id)
		{
			return await _context.Wages.AsTracking().FirstOrDefaultAsync(x => x.Id == id);
		}

		public async Task<List<Wage>> GetListAsync()
		{
			return await _context.Wages.ToListAsync();
		}

		public async Task<int> SaveChangesAsync()
		{
			return await _context.SaveChangesAsync();
		}

		public void Dispose()
		{
			_context?.Dispose();
		}
	}
}
