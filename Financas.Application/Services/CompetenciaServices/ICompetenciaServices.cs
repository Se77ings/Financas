using Financeiro.Business.Models.CompetenciaModels;
using Financeiro.Business.ResultPattern;

namespace Financas.Application.Services.CompetenciaServices
{
	public interface ICompetenciaServices
	{
		Task<(Result<bool> result, List<CompetenciaSummary> lista)> GetListAsync();
		Task<(Result<bool> result, CompetenciaView view)> GetByIdAsync(Guid id);
		Task<Result<bool>> AddAsync(CompetenciaRequest request);
		Task<Result<bool>> UpdateAsync(Guid id, CompetenciaRequest request);
		Task<Result<bool>> DeleteAsync(Guid id);
		//Task<bool> ExistsAsync(string mes, int ano);

	}
}
