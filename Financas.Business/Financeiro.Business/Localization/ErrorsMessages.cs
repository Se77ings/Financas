namespace Financeiro.Business.Localization
{
	public static class ErrorMessages
	{
		public static string RecordNotFoundFormat(string entityName) => string.Format(Errors.RecordNotFound, entityName);

	}
}
