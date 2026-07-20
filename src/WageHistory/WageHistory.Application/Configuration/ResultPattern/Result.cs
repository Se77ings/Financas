namespace WageHistory.Business.Shared.ResultPattern
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

		public TResult Map<TResult>(Func<T, TResult> onSuccess, Func<Error, TResult> onFailure)
		{
			return IsSuccess ? onSuccess(Value!) : onFailure(Error!);
		}
	}
}
