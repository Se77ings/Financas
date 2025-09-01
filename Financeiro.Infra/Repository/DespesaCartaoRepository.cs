using Financeiro.Business.Aggregates.Obrigação;
using Financeiro.Business.Interfaces;
using Financeiro.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace Financeiro.Infra.Repository
{
	public class DespesaCartaoRepository : IDespesaCartaoRepository
	{
		private readonly FinanceiroDBContext _context;
		public DespesaCartaoRepository(FinanceiroDBContext context)
		{
			_context = context;
		}
		public async Task AddAsync(DespesaCartao entidade)
		{
			_context.DespesasCartao.Add(entidade);
			await SaveChangesAsync();
		}


		public async Task UpdateAsync(DespesaCartao entidade)
		{
			_context.DespesasCartao.Update(entidade);
			await SaveChangesAsync();
		}

		public async Task RemoveAsync(DespesaCartao entidade)
		{
			_context.DespesasCartao.Remove(entidade);
			await SaveChangesAsync();

		}

		public async Task<DespesaCartao> GetByIdAsync(Guid id)
		{
			return await _context.DespesasCartao.FindAsync(id);
		}

		public async Task<List<DespesaCartao>> GetListAsync()
		{
			return await _context.DespesasCartao.ToListAsync();
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
