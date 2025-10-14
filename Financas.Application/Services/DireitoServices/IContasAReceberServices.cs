using Financeiro.Business.Models.DireitoModels;
using Financeiro.Business.Shared.ResultPattern;

namespace Financas.Application.Services.DireitoServices
{
	public interface IContasAReceberServices
	{
		public Task<(Result<bool> result, List<ContasAReceberSummary> lista)> GetListAsync();

		public Task<(Result<bool> result, ContasAReceberView view)> GetByIdAsync(Guid id);

		public Task<Result<bool>> AddAsync(ContasAReceberRequest request);
		public Task<Result<bool>> UpdateAsync(Guid id, ContasAReceberRequest request);
		public Task<Result<bool>> DeleteAsync(Guid id);
		//public Task<bool> ExistsAsync(string descricao, decimal total, DateTime data); // nao proibitivo, apenas informativo.
	}
}
