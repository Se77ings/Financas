namespace Financeiro.Business.Aggregates
{
	public abstract class Entidade
	{
		public Guid Id { get; set; }
		public DateTime CriadoEm { get; private set; }

		public Entidade()
		{
			Id = Guid.NewGuid();
			CriadoEm = DateTime.Now;
		}
	}
}
