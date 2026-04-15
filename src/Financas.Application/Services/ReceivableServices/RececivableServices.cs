using Financas.Business.Shared.ServiceBase;
using Financeiro.Application.Models.ReceivableModels;
using Financeiro.Business.Aggregates.Interfaces;
using Financeiro.Business.Aggregates.Receivable;
using Financeiro.Business.Aggregates.Validações;
using Financeiro.Business.Shared.Localization;
using Financeiro.Business.Shared.ResultPattern;

namespace Financas.Application.Services.DireitoServices
{
	public class RececivableServices : ServiceBase, ReceivableServices
	{
		private readonly IReceivableRepository _repository;
		public RececivableServices(IReceivableRepository repository)
		{
			_repository = repository;
		}

		public async Task<Result<bool>> AddAsync(ReceivableRequest request)
		{
			//TODO: Verificar se já existe essa entidade.
			var contaAReceber = new Receivable(request.Description, request.SubTotal, request.Date, request.Discounts, request.EffectiveDateId);

			var validation = await ValidateEntityAsync<Receivable, ReceivableValidator>(contaAReceber);

			if (!validation.Success)
				return Result<bool>.Failure(validation.Error);

			await _repository.AddAsync(contaAReceber);

			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> UpdateAsync(Guid id, ReceivableRequest request)
		{
			var contaAReceber = await GetReceivableById(id);

			var novaEntidade = new Receivable(request.Description, request.SubTotal, request.Date, request.Discounts, request.EffectiveDateId);
			//TODO: Verificar se já existe a nova entidade ou homônima.

			var validation = await ValidateEntityAsync<Receivable, ReceivableValidator>(novaEntidade);
			if (!validation.Success)
				return Result<bool>.Failure(validation.Error);

			contaAReceber.Update(request.Description, request.SubTotal, request.Date, request.Discounts, request.EffectiveDateId);

			await _repository.UpdateAsync(contaAReceber);

			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> DeleteAsync(Guid id)
		{
			var contaAReceber = await GetReceivableById(id);

			await _repository.RemoveAsync(contaAReceber);

			return Result<bool>.Success(true);
		}

		public async Task<(Result<bool> result, ReceivableView view)> GetByIdAsync(Guid id)
		{
			var contaAReceber = await GetReceivableById(id);


			return (Result<bool>.Success(true), (ReceivableView)contaAReceber);

		}

		public async Task<(Result<bool> result, List<ReceivableSummary> lista)> GetListAsync()
		{
			var list = await _repository.GetListAsync();

			if (list == null)
				return (Result<bool>.Failure(new Error(Errors.EmptyData)), null);
 
			var result = list.Select(c => new ReceivableSummary { Id = c.Id, Date = c.Date, Description = c.Description, Total = c.Total }).ToList();

			return (Result<bool>.Success(true), result);
		}

		#region Métodos internos

		public async Task<Receivable> GetReceivableById(Guid id)
		{
			return await _repository.GetByIdAsync(id);

		}

		#endregion
	}
}
