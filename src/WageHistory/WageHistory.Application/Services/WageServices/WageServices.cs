using WageHistory.Application.Models.WageModels;
using WageHistory.Business.Aggregates;
using WageHistory.Business.Aggregates.Interfaces;
using WageHistory.Business.Shared.ResultPattern;
using WageHistory.Business.Shared.ServiceBase;

namespace WageHistory.Application.Services.WageServices
{
	public class WageServices : ServiceBase, IWageServices
	{
		private readonly IWageRepository _repository;

		public WageServices(IWageRepository repository)
		{
			_repository = repository;
		}

		public async Task<Result<bool>> AddAsync(WageRequest request)
		{
			var wage = new Wage(request.GrossValue, request.GrossValue, request.PaymentDate);

			foreach (var deduction in request.Deductions)
			{
				wage.AddDeduction(deduction.Description, deduction.Value);
			}

			await _repository.AddAsync(wage);
			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> UpdateAsync(Guid id, WageRequest request)
		{
			var wage = await _repository.GetByIdAsync(id);
			if (wage == null)
				return Result<bool>.Failure(new Error("Wage not found"));

			wage.Update(request.GrossValue, request.PaymentDate);
			wage.ClearDeductions();

			foreach (var deduction in request.Deductions)
			{
				wage.AddDeduction(deduction.Description, deduction.Value);
			}

			await _repository.UpdateAsync(wage);
			return Result<bool>.Success(true);
		}

		public async Task<Result<bool>> DeleteAsync(Guid id)
		{
			var wage = await _repository.GetByIdAsync(id);
			if (wage == null)
				return Result<bool>.Failure(new Error("Wage not found"));

			await _repository.RemoveAsync(wage);
			return Result<bool>.Success(true);
		}

		public async Task<(Result<bool> result, WageView? view)> GetByIdAsync(Guid id)
		{
			var wage = await _repository.GetByIdAsync(id);
			if (wage == null)
				return (Result<bool>.Failure(new Error("Wage not found")), null);

			return (Result<bool>.Success(true), (WageView)wage);
		}

		public async Task<(Result<bool> result, List<WageSummary> lista)> GetListAsync()
		{
			var list = await _repository.GetListAsync();
			if (list == null)
				return (Result<bool>.Failure(new Error("No data found")), new List<WageSummary>());

			var result = list.Select(w => new WageSummary
			{
				Id = w.Id,
				GrossValue = w.GrossValue,
				NetValue = w.NetValue,
				PaymentDate = w.PaymentDate
			}).ToList();

			return (Result<bool>.Success(true), result);
		}
	}
}
