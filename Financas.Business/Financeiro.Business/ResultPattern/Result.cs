namespace Financeiro.Business.ResultPattern
{
	public class Result<T>
	{
		public T? Value { get; }
		public Error? Error { get; }
		public bool IsSuccess => Error == null;

		private Result(T value)
		{
			Value = value;
			Error = null;
		}

		private Result(Error error)
		{
			Value = default;
			Error = error;
		}

		public static Result<T> Success(T value) => new(value);
		public static Result<T> Failure(Error error) => new(error);

		public TResult Map<TResult> (Func<T, TResult> onSuccess, Func<Error, TResult> onFailure)
		{
			return IsSuccess? onSuccess(Value!) : onFailure(Error!);
		}

		// Exemplo de uso na Service:
		// return Result<T>.Failure(new Error("Erro ao processar a requisição", "ERRO_PROCESSAMENTO"));
		// return Result<T>.Success(entity)

		//Exemplo de uso na Controller:

		// var result = await _service.GetByIdAsync(id); --> Essa var recebe o Result
		// return result.Map<IActionResult>(
		//     onSuccess: result => Ok(entity),
		//     onFailure: error => BadRequest(error.Message)
	}
}
