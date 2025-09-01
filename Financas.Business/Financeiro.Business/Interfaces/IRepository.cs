using Financeiro.Business.Aggregates;

namespace Financeiro.Business.Interfaces
{
	public interface IRepository<TEntity> : IDisposable where TEntity : Entidade
	{
		Task AddAsync(TEntity entidade);
		Task<TEntity> GetByIdAsync(Guid id);
		Task<List<TEntity>> GetListAsync();
		Task UpdateAsync(TEntity entidade);
		Task RemoveAsync(TEntity entidade);
		Task<int> SaveChangesAsync();
	}
}
