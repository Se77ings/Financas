using Financas.Application.Services.ObrigaçãoServices.Interfaces;
using Financeiro.Business.Aggregates.Obrigação;
using Financeiro.Business.Aggregates.Validações;
using Financeiro.Business.Interfaces;
using Financeiro.Business.Localization;
using Financeiro.Business.Models.ObrigaçãoModels;
using Financeiro.Business.ResultPattern;
using FluentValidation.Results;

namespace Financas.Application.Services.ObrigaçãoServices
{
	public class DespesaCartaoServices : IDespesaCartaoServices
	{
		private readonly IDespesaCartaoRepository _repository;

		public DespesaCartaoServices(IDespesaCartaoRepository repository)
		{
			_repository = repository;
		}

		public async Task<Result<bool>> AddAsync(DespesaCartaoRequest request)
		{
			var entidade = new DespesaCartao(request.Descricao, request.SubTotal, request.DataVencimento, request.Descontos, request.Observacao, request.CompetenciaId);
			if (!await DespesaCartaoIsValid(entidade))
				return Result<bool>.Failure(new Error(Errors.InvalidEntity));

			await _repository.AddAsync(entidade);
			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> UpdateAsync(Guid id, DespesaCartaoRequest request)
		{
			var result = await ObterDespesaCartaoPorId(id);

			var novaEntidade = new DespesaCartao(request.Descricao, request.SubTotal, request.DataVencimento, request.Descontos, request.Observacao, request.CompetenciaId);

			if (!await DespesaCartaoIsValid(novaEntidade))
				return Result<bool>.Failure(new Error(Errors.InvalidEntity));

			result.Atualizar(request.Descricao, request.SubTotal, request.DataVencimento, request.Descontos, request.Observacao, request.CompetenciaId);

			await _repository.UpdateAsync(result);
			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> DeleteAsync(Guid id)
		{
			var result = await ObterDespesaCartaoPorId(id);

			await _repository.RemoveAsync(result);
			return Result<bool>.Success(true);
		}

		public async Task<(Result<bool> result, List<DespesaCartaoView> lista)> GetListAsync()
		{
			var list = await _repository.GetListAsync();

			return (Result<bool>.Success(true), list.Select(x => (DespesaCartaoView)x).ToList());
		}
		public async Task<(Result<bool> result, DespesaCartaoView view)> GetByIdAsync(Guid id)
		{
			var despesaResult = await ObterDespesaCartaoPorId(id);

			return (Result<bool>.Success(true), (DespesaCartaoView)despesaResult);
		}

		public async Task<bool> AddItemAsync(Guid id, DespesaCartaoItemRequest request)
		{
			var result = await ObterDespesaCartaoPorId(id);

			result.AdicionarDespesa(request.Descricao, request.SubTotal, request.DataVencimento, request.Descontos, request.Observacao, request.ParcelaAtual, request.QtdParcelas, result.CompetenciaId);

			await _repository.UpdateAsync(result);
			return true;
		}

		public async Task<bool> UpdateItemAsync(Guid id, Guid itemId, DespesaCartaoItemRequest request)
		{
			var result = await ObterDespesaCartaoPorId(id);

			result.AtualizarDespesa(itemId, request.Descricao, request.SubTotal, request.DataVencimento, request.Descontos, request.Observacao, request.ParcelaAtual, request.QtdParcelas);

			await _repository.UpdateAsync(result);
			return true;
		}

		public async Task<bool> RemoveItemAsync(Guid id, Guid itemId)
		{
			var result = await ObterDespesaCartaoPorId(id);

			result.RemoverDespesa(itemId);

			await _repository.UpdateAsync(result);
			return true;
		}

		#region Métodos internos

		private async Task<DespesaCartao> ObterDespesaCartaoPorId(Guid id)
		{
			return await _repository.GetByIdAsync(id);

		}

		private async Task<bool> DespesaCartaoIsValid(DespesaCartao entidade)
		{
			DespesaCartaoValidador validator = new();
			ValidationResult result = await validator.ValidateAsync(entidade);

			return result.IsValid;
		}

		#endregion
	}
}
