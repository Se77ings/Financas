namespace Financeiro.Business.Aggregates.Interfaces
{
	public interface ICompetenciaRepository : IRepository<Competencia>
	{
		Task<bool> ExistsAsync(string mes, int ano);
		Task<bool> ExistsOtherAsync(Guid id, string mes, int ano);

	}
}
