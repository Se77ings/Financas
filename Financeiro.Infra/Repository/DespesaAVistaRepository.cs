using Financeiro.Business.Aggregates.Obrigação;
using Financeiro.Business.Interfaces;
using Financeiro.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace Financeiro.Infra.Repository
{
	public class DespesaAVistaRepository : IDespesaAVistaRepository
	{
		private readonly FinanceiroDBContext _context;
		public DespesaAVistaRepository(FinanceiroDBContext context)
		{
			_context = context;
		}

		public async Task AddAsync(DespesaAVista entidade)
		{
			_context.DespesasAVista.Add(entidade);
			await SaveChangesAsync();

		}
		public async Task UpdateAsync(DespesaAVista entidade)
		{
			_context.DespesasAVista.Update(entidade);
			await SaveChangesAsync();
		}
		public async Task RemoveAsync(DespesaAVista entidade)
		{
			_context.DespesasAVista.Remove(entidade);
			await SaveChangesAsync();
		}
		public async Task<DespesaAVista> GetByIdAsync(Guid id)
		{
			return await _context.DespesasAVista.FindAsync(id);
		}

		public async Task<List<DespesaAVista>> GetListAsync()
		{
			return await _context.DespesasAVista.ToListAsync();

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
