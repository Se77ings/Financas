namespace Financeiro.Business.Aggregates.Interfaces
{
	public interface IReceivableRepository : IRepository<Receivable.Receivable>
	{
		Task<bool> ExistsAsync(string description, DateOnly date);
		Task<bool> ExistsOtherAsync(Guid id,string description, DateOnly date);
	}
}
