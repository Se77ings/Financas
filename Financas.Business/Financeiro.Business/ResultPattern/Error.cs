namespace Financeiro.Business.ResultPattern
{
	public class Error
	{
		public string Message { get; }
		public string Code { get; }

		public Error(string message, string code = null)
		{
			Message = message;
			Code = code;
		}

		public Error()
		{

		}
	}
}
