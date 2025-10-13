using Financas.Application.Services.ObrigaçãoServices.Interfaces;
using Financas.Business.Shared.ServiceBase;
using Financeiro.Business.Aggregates.Interfaces;
using Financeiro.Business.Aggregates.Obrigação;
using Financeiro.Business.Aggregates.Validações;
using Financeiro.Business.Models.ObrigaçãoModels;
using Financeiro.Business.Shared.ResultPattern;

namespace Financas.Application.Services.ObrigaçãoServices
{
	public class DespesaAVistaServices : ServiceBase, IDespesaAVistaServices
	{
		private readonly IDespesaAVistaRepository _repository;

		public DespesaAVistaServices(IDespesaAVistaRepository repository)
		{
			_repository = repository;
		}

		public async Task<Result<bool>> AddAsync(DespesaAVistaRequest request)
		{
			// TODO: Verificar se já existe essa entidade.
			var despesa = new DespesaAVista(request.Descricao, request.SubTotal, request.DataVencimento, request.Descontos, request.Observacao, request.CompetenciaId);

			var validation = await ValidateEntityAsync<DespesaAVista, DespesaAVistaValidator>(despesa);
			if (!validation.Success)
				return Result<bool>.Failure(validation.Error);

			await _repository.AddAsync(despesa);
			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> UpdateAsync(Guid id, DespesaAVistaRequest request)
		{
			var despesaResult = await ObterDespesaAVistaPorId(id);

			var novaEntidade = new DespesaAVista(request.Descricao, request.SubTotal, request.DataVencimento, request.Descontos, request.Observacao, request.CompetenciaId);
			// TODO: Verificar se já existe a nova entidade ou homônima.

			var validation = await ValidateEntityAsync<DespesaAVista, DespesaAVistaValidator>(novaEntidade);
			
			if (!validation.Success)
				return Result<bool>.Failure(validation.Error);

			despesaResult.Atualizar(request.Descricao, request.SubTotal, request.DataVencimento, request.Descontos, request.Observacao, request.CompetenciaId);

			await _repository.UpdateAsync(despesaResult);

			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> DeleteAsync(Guid id)
		{
			var despesaResult = await ObterDespesaAVistaPorId(id);

			await _repository.RemoveAsync(despesaResult);

			return Result<bool>.Success(true);
		}

		public async Task<(Result<bool> result, DespesaAVistaView view)> GetByIdAsync(Guid id)
		{
			var despesaResult = await ObterDespesaAVistaPorId(id);

			return (Result<bool>.Success(true), (DespesaAVistaView)despesaResult);
		}

		public async Task<(Result<bool> result, List<DespesaAVistaView> data)> GetListAsync()
		{
			var list = await _repository.GetListAsync();

			return (Result<bool>.Success(true), list.Select(x => (DespesaAVistaView)x).ToList());
		}

		#region Métodos internos

		private async Task<DespesaAVista> ObterDespesaAVistaPorId(Guid id)
		{
			return await _repository.GetByIdAsync(id);
		}



		#endregion
	}
}
