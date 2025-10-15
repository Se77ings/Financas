namespace Financeiro.Business.Aggregates.Interfaces
{
	public interface IEffectiveDateRepository : IRepository<EffectiveDate>
	{
		Task<bool> ExistsAsync(string month, int year);
		Task<bool> ExistsOtherAsync(Guid id, string month, int year);

	}
}
