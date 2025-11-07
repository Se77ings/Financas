using Financeiro.Business.Shared.ResultPattern;
using FluentValidation;
using FluentValidation.Results;

namespace Financas.Business.Shared.ServiceBase
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
