using Financeiro.Business.Shared;

namespace Financeiro.Business.Interfaces
{
	public interface IRepository<TEntity> : IDisposable where TEntity : Entity
	{
		Task AddAsync(TEntity entity);
		Task<TEntity> GetByIdAsync(Guid id);
		Task<List<TEntity>> GetListAsync();
		Task UpdateAsync(TEntity entity);
		Task RemoveAsync(TEntity entity);
		Task<int> SaveChangesAsync();
	}
}
