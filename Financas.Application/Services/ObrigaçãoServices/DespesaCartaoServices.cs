using Financas.Application.Services.ObrigaçãoServices.Interfaces;
using Financas.Business.Shared.ServiceBase;
using Financeiro.Business.Aggregates.Interfaces;
using Financeiro.Business.Aggregates.Obrigação;
using Financeiro.Business.Aggregates.Validações;
using Financeiro.Business.Models.ObrigaçãoModels;
using Financeiro.Business.Shared.ResultPattern;
using static Financeiro.Business.Aggregates.Obrigação.DespesaCartao;

namespace Financas.Application.Services.ObrigaçãoServices
{
	public class DespesaCartaoServices : ServiceBase, IDespesaCartaoServices
	{
		private readonly IDespesaCartaoRepository _repository;

		public DespesaCartaoServices(IDespesaCartaoRepository repository)
		{
			_repository = repository;
		}

		public async Task<Result<bool>> AddAsync(DespesaCartaoRequest request)
		{
			var entidade = new DespesaCartao(request.Descricao, request.SubTotal, request.DataVencimento, request.Descontos, request.Observacao, request.CompetenciaId);

			var validation = await ValidateEntityAsync<DespesaCartao, DespesaCartaoValidator>(entidade);

			if (!validation.Success)
				return Result<bool>.Failure(validation.Error);

			await _repository.AddAsync(entidade);
			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> UpdateAsync(Guid id, DespesaCartaoRequest request)
		{
			var result = await ObterDespesaCartaoPorId(id);

			var novaEntidade = new DespesaCartao(request.Descricao, request.SubTotal, request.DataVencimento, request.Descontos, request.Observacao, request.CompetenciaId);

			var validation = await ValidateEntityAsync<DespesaCartao, DespesaCartaoValidator>(novaEntidade);

			if (!validation.Success)
				return Result<bool>.Failure(validation.Error);

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

		public async Task<Result<bool>> AddItemAsync(Guid id, DespesaCartaoItemRequest request)
		{ // TODO: Validar..
			var cartao = await ObterDespesaCartaoPorId(id);

			var despesa = new Despesa(request.Descricao, request.SubTotal, request.DataVencimento, request.Descontos, request.Observacao, request.ParcelaAtual, request.QtdParcelas, cartao.CompetenciaId, cartao.Id);

			await _repository.AddItemAsync(despesa);

			cartao.AdicionarDespesa(despesa);

			await _repository.UpdateAsync(cartao);

			await _repository.SaveChangesAsync();

			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> UpdateItemAsync(Guid id, Guid itemId, DespesaCartaoItemRequest request)
		{
			var result = await ObterDespesaCartaoPorId(id);

			result.AtualizarDespesa(itemId, request.Descricao, request.SubTotal, request.DataVencimento, request.Descontos, request.Observacao, request.ParcelaAtual, request.QtdParcelas);

			await _repository.UpdateAsync(result);
			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> RemoveItemAsync(Guid id, Guid itemId)
		{
			var result = await ObterDespesaCartaoPorId(id);

			result.RemoverDespesa(itemId);

			await _repository.UpdateAsync(result);
			return Result<bool>.Success(true);
		}

		#region Métodos internos

		private async Task<DespesaCartao> ObterDespesaCartaoPorId(Guid id)
		{
			return await _repository.GetByIdAsync(id);

		}

		#endregion
	}
}
