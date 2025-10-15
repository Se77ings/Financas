namespace Financeiro.Business.Aggregates
{
	public abstract class Entity
	{
		public Guid Id { get; set; }
		public DateTime CreatedAt { get; private set; }

		public Entity()
		{
			Id = Guid.NewGuid();
			CreatedAt = DateTime.Now;
		}
	}
}
