using Financeiro.Business.Aggregates.Direito;
using Financeiro.Business.Interfaces;
using Financeiro.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace Financeiro.Infra.Repository
{
	public class ContasAReceberRepository : IContasAReceberRepository
	{
		private readonly FinanceiroDBContext _context;

		public ContasAReceberRepository(FinanceiroDBContext context)
		{
			_context = context;
		}
		public async Task AddAsync(ContasAReceber entidade)
		{
			_context.ContasAReceber.Add(entidade);
			await SaveChangesAsync();
		}

		public async Task UpdateAsync(ContasAReceber entidade)
		{
			_context.ContasAReceber.Update(entidade);
			await SaveChangesAsync();
		}

		public async Task RemoveAsync(ContasAReceber entidade)
		{
			_context.ContasAReceber.Remove(entidade);
			await SaveChangesAsync();
		}

		public async Task<ContasAReceber> GetByIdAsync(Guid id)
		{
			return await _context.ContasAReceber.FindAsync(id);
		}

		public async Task<List<ContasAReceber>> GetListAsync()
		{
			return await _context.ContasAReceber.ToListAsync();
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
