using Financas.Application.Services.ObrigaçãoServices.Interfaces;
using Financeiro.Business.Aggregates.Obrigação;
using Financeiro.Business.Aggregates.Validações;
using Financeiro.Business.Interfaces;
using Financeiro.Business.Localization;
using Financeiro.Business.Models.CompetenciaModels;
using Financeiro.Business.Models.ObrigaçãoModels;
using Financeiro.Business.ResultPattern;
using FluentValidation.Results;

namespace Financas.Application.Services.ObrigaçãoServices
{
	public class DespesaAVistaServices : IDespesaAVistaServices
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

			if (!await DespesaAVistaIsValid(despesa))
				return Result<bool>.Failure(new Error(Errors.InvalidEntity));

			await _repository.AddAsync(despesa);
			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> UpdateAsync(Guid id, DespesaAVistaRequest request)
		{
			var despesaResult = await ObterDespesaAVistaPorId(id);

			var novaEntidade = new DespesaAVista(request.Descricao, request.SubTotal, request.DataVencimento, request.Descontos, request.Observacao, request.CompetenciaId);
			// TODO: Verificar se já existe a nova entidade ou homônima.

			if (!await DespesaAVistaIsValid(novaEntidade))
				return Result<bool>.Failure(new Error(Errors.InvalidEntity));

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

		private async Task<bool> DespesaAVistaIsValid(DespesaAVista entidade)
		{
			DespesaAVistaValidador validator = new();
			ValidationResult result = await validator.ValidateAsync(entidade);

			if (!result.IsValid)
				return false;

			return true;
		}

		private async Task<DespesaAVista> ObterDespesaAVistaPorId(Guid id)
		{
			return await _repository.GetByIdAsync(id);
		}



		#endregion
	}
}
