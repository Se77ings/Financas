using Financeiro.Business.Aggregates.Interfaces;
using Financeiro.Business.Aggregates.Obrigação;
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
			_context.ContasAPagar.Add(entidade);
			await SaveChangesAsync();

		}
		public async Task UpdateAsync(DespesaAVista entidade)
		{
			_context.ContasAPagar.Update(entidade);
			await SaveChangesAsync();
		}
		public async Task RemoveAsync(DespesaAVista entidade)
		{
			_context.ContasAPagar.Remove(entidade);
			await SaveChangesAsync();
		}
		public async Task<DespesaAVista> GetByIdAsync(Guid id)
		{
			return await _context.ContasAPagar.OfType<DespesaAVista>().FirstOrDefaultAsync(x=> x.Id == id);
		}

		public async Task<List<DespesaAVista>> GetListAsync()
		{
			return await _context.ContasAPagar.OfType<DespesaAVista>().ToListAsync();

		}
		public async Task<int> SaveChangesAsync()
		{
			return await _context.SaveChangesAsync();
		}

		public void Dispose()
		{
			_context?.Dispose();
		}

		public async Task<bool> ExistsAsync(string descricao, DateTime date)
		{
			return await _context.ContasAPagar.AnyAsync(x => x.Descricao == descricao && x.DataVencimento == date);
		}

		public async Task<bool> ExistsOtherAsync(Guid id, string descricao, DateTime date)
		{
			return await _context.ContasAPagar.AnyAsync(x => x.Descricao == descricao && x.DataVencimento == date && x.Id != id);
		}
	}
}
