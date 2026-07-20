using FluentValidation;
using FluentValidation.Results;
using WageHistory.Business.Shared.ResultPattern;

namespace WageHistory.Business.Shared.ServiceBase
{
	public abstract class ServiceBase
	{
		protected async Task<(bool Success, Error Error)> ValidateEntityAsync<TEntity, TValidator>(TEntity entity)
			where TEntity : class
			where TValidator : AbstractValidator<TEntity>, new()
		{
			TValidator validator = new TValidator();
			ValidationResult result = await validator.ValidateAsync(entity);

			if (!result.IsValid)
			{
				Error error = new(result.ToString("\n"));
				return (false, error);
			}
			return (true, new Error());
		}
	}
}
