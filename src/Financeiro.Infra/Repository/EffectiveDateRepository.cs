using Financeiro.Business.Aggregates;
using Financeiro.Business.Aggregates.Interfaces;
using Financeiro.Business.Shared;
using Financeiro.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace Financeiro.Infra.Repository
{
	public class EffectiveDateRepository : IEffectiveDateRepository
	{
		private readonly FinanceiroDBContext _context;
		public EffectiveDateRepository(FinanceiroDBContext context)
		{
			_context = context;
		}

		public async Task AddAsync(EffectiveDate effectiveDate)
		{
			_context.EffectiveDate.Add(effectiveDate);
			await SaveChangesAsync();
		}
		public async Task UpdateAsync(EffectiveDate effectiveDate)
		{
			_context.EffectiveDate.Update(effectiveDate);
			await SaveChangesAsync();
		}

		public async Task RemoveAsync(EffectiveDate effectiveDate)
		{
			_context.EffectiveDate.Remove(effectiveDate);
			await SaveChangesAsync();

		}

		public async Task<EffectiveDate> GetByIdAsync(Guid id)
		{
			return await _context.EffectiveDate
				.Include(x => x.Receivable)
				.Include(x=> x.Payable)
				.FirstOrDefaultAsync(x => x.Id == id);
		}

		public async Task<List<EffectiveDate>> GetListAsync()
		{
			// incluir coleções filhas para permitir cálculos e exibição de informações resumo
			return await _context.EffectiveDate
				.Include(x => x.Receivable)
				.Include(x => x.Payable)
				.ToListAsync();
		}

		public async Task<int> SaveChangesAsync()
		{
			return await _context.SaveChangesAsync();
		}

		public void Dispose()
		{
			_context?.Dispose();
		}

		public async Task<bool> ExistsAsync(CalendarMonth month, int year)
		{
			return await _context.EffectiveDate.AnyAsync(x => x.Month == month && x.Year == year);
		}

		public async Task<bool> ExistsOtherAsync(Guid id, CalendarMonth month, int year)
		{
			return await _context.EffectiveDate.AnyAsync(x => x.Month == month && x.Year == year && x.Id != id);
		}
	}
}
