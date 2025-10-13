using Financeiro.Business.Models.ObrigaçãoModels;
using Financeiro.Business.Shared.ResultPattern;

namespace Financas.Application.Services.ObrigaçãoServices.Interfaces
{
	public interface IDespesaAVistaServices
	{
		public Task<(Result<bool> result, DespesaAVistaView view)> GetByIdAsync(Guid id);
		public Task<(Result<bool> result, List<DespesaAVistaView> data)> GetListAsync();
		public Task<Result<bool>> AddAsync(DespesaAVistaRequest request);
		public Task<Result<bool>> UpdateAsync(Guid id, DespesaAVistaRequest request);
		public Task<Result<bool>> DeleteAsync(Guid id);
	}
}
