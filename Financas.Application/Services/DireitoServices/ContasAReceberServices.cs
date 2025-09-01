using Financeiro.Business.Aggregates.Direito;
using Financeiro.Business.Aggregates.Validações;
using Financeiro.Business.Interfaces;
using Financeiro.Business.Localization;
using Financeiro.Business.Models.DireitoModels;
using Financeiro.Business.ResultPattern;
using FluentValidation.Results;

namespace Financas.Application.Services.DireitoServices
{ // quando voltar, pegar a sugestão do Gemini, de TV, TE p/ o validador
	public class ContasAReceberServices : IContasAReceberServices
	{
		private readonly IContasAReceberRepository _repository;
		public ContasAReceberServices(IContasAReceberRepository repository)
		{
			_repository = repository;
		}

		public async Task<Result<bool>> AddAsync(ContasAReceberRequest request)
		{
			//TODO: Verificar se já existe essa entidade.
			var contaAReceber = new ContasAReceber(request.Descricao, request.SubTotal, request.Data, request.Descontos, request.CompetenciaId);

			var validation = await ContasAReceberIsValid(contaAReceber);
			if (!validation.Success)
				return Result<bool>.Failure(validation.Error);

			await _repository.AddAsync(contaAReceber);

			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> UpdateAsync(Guid id, ContasAReceberRequest request)
		{
			var contaAReceber = await ObterContaAReceberPorId(id);

			var novaEntidade = new ContasAReceber(request.Descricao, request.SubTotal, request.Data, request.Descontos, request.CompetenciaId);
			//TODO: Verificar se já existe a nova entidade ou homônima.

			var validation = await ContasAReceberIsValid(novaEntidade);
			if (!validation.Success)
				return Result<bool>.Failure(validation.Error);

			contaAReceber.Atualizar(request.Descricao, request.SubTotal, request.Data, request.Descontos, request.CompetenciaId);

			await _repository.UpdateAsync(contaAReceber);

			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> DeleteAsync(Guid id)
		{
			var contaAReceber = await ObterContaAReceberPorId(id);

			await _repository.RemoveAsync(contaAReceber);

			return Result<bool>.Success(true);
		}

		public async Task<(Result<bool> result, ContasAReceberView view)> GetByIdAsync(Guid id)
		{
			var contaAReceber = await ObterContaAReceberPorId(id);


			return (Result<bool>.Success(true), (ContasAReceberView)contaAReceber);

		}

		public async Task<(Result<bool> result, List<ContasAReceberSummary> lista)> GetListAsync()
		{
			var list = await _repository.GetListAsync();

			if (list == null)
				return (Result<bool>.Failure(new Error(Errors.EmptyData)), null);
			// TODO: Mudar para implicit operator
			var result = list.Select(c => new ContasAReceberSummary { Id = c.Id, Data = c.Data, Descricao = c.Descricao, Total = c.Total }).ToList();

			return (Result<bool>.Success(true), result);
		}

		#region Métodos internos
		public async Task<(bool Success, Error Error)> ContasAReceberIsValid(ContasAReceber entidade)
		{
			ContasAReceberValidador validator = new();
			ValidationResult result = await validator.ValidateAsync(entidade);

			if (!result.IsValid)
			{
				Error error = new(result.ToString("\n"));
				return (false, error);

			}
			return (true, new Error());
		}

		public async Task<ContasAReceber> ObterContaAReceberPorId(Guid id)
		{
			return await _repository.GetByIdAsync(id);

		}

		#endregion
	}
}
