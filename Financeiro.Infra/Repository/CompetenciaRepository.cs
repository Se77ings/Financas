using Financeiro.Business.Aggregates;
using Financeiro.Business.Aggregates.Interfaces;
using Financeiro.Business.Aggregates.Obrigação;
using Financeiro.Infra.Context;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace Financeiro.Infra.Repository
{
	public class CompetenciaRepository : ICompetenciaRepository
	{
		private readonly FinanceiroDBContext _context;
		public CompetenciaRepository(FinanceiroDBContext context)
		{
			_context = context;
		}

		public async Task AddAsync(Competencia competencia)
		{
			_context.Competencias.Add(competencia);
			await SaveChangesAsync();
		}
		public async Task UpdateAsync(Competencia competencia)
		{
			_context.Competencias.Update(competencia);
			await SaveChangesAsync();
		}

		public async Task RemoveAsync(Competencia competencia)
		{
			_context.Competencias.Remove(competencia);
			await SaveChangesAsync();

		}

		public async Task<Competencia> GetByIdAsync(Guid id)
		{
			return await _context.Competencias
				.Include(x => x.ContasAReceber)
				.Include(x=> x.ContasAPagar)
				.FirstOrDefaultAsync(x => x.Id == id);
		}

		public async Task<List<Competencia>> GetListAsync()
		{
			return await _context.Competencias.ToListAsync();
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
