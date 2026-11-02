namespace Financeiro.Business.Shared
{
	public abstract class Entity
	{
		public Guid Id { get; private set; }
		public DateTime CreatedAt { get; private set; }

		public Entity()
		{
			Id = Guid.NewGuid();
			CreatedAt = DateTime.Now;
		}
	}
}
