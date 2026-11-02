namespace Financeiro.Business.Shared.ResultPattern
{
	public class Error
	{
		private const string NotFoundCode = "not_found";

		public string Message { get; }
		public string? Code { get; }

		/// <summary>O registro pedido não existe; a API responde 404 em vez de 400.</summary>
		public bool IsNotFound => Code == NotFoundCode;

		public Error(string message, string? code = null)
		{
			Message = message;
			Code = code;
		}

		public static Error NotFound(string message) => new(message, NotFoundCode);
	}
}
