using Financeiro.Business.Aggregates.Interfaces;
using Financeiro.Business.Aggregates.Obrigação;
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
			_context.ContasAPagar.Add(entidade);
			await SaveChangesAsync();
		}


		public async Task UpdateAsync(DespesaCartao entidade)
		{
			_context.ContasAPagar.Update(entidade);
			await SaveChangesAsync();
		}

		public async Task RemoveAsync(DespesaCartao entidade)
		{
			_context.ContasAPagar.Remove(entidade);
			await SaveChangesAsync();

		}

		public async Task<DespesaCartao> GetByIdAsync(Guid id)
		{
			return await _context.ContasAPagar.OfType<DespesaCartao>().Include(x => x.Despesas).FirstOrDefaultAsync(x => x.Id == id);// ja tirei o astracking dai
		}

		public async Task<List<DespesaCartao>> GetListAsync()
		{
			return await _context.ContasAPagar.OfType<DespesaCartao>().ToListAsync();
		}
		public async Task<int> SaveChangesAsync()
		{
			return await _context.SaveChangesAsync();
		}

		public async Task AddItemAsync(DespesaCartao.Despesa item)
		{
			_context.DespesasCartaoItem.Add(item);
		}

		public void Dispose()
		{
			_context?.Dispose();
		}
	}
}
