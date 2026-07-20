using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WageHistory.Infra.Context
{
	public class WageHistoryDBContextFactory : IDesignTimeDbContextFactory<WageHistoryDBContext>
	{
		public WageHistoryDBContext CreateDbContext(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<WageHistoryDBContext>();
			
			// Conexão usada para fins de design-time/migrations
			optionsBuilder.UseSqlServer("Server=Gabriel;Database=WageHistory;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true");

			return new WageHistoryDBContext(optionsBuilder.Options);
		}
	}
}
