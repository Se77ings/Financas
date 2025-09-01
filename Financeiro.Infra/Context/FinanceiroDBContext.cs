using Financeiro.Business.Aggregates;
using Financeiro.Business.Aggregates.Direito;
using Financeiro.Business.Aggregates.Obrigação;
using Microsoft.EntityFrameworkCore;

namespace Financeiro.Infra.Context
{
	public class FinanceiroDBContext : DbContext
	{
		/// <summary>
		/// Contas a Receber
		/// </summary>
		public DbSet<ContasAReceber> ContasAReceber { get; set; }
		/// <summary>
		/// Contas A Pagar
		/// </summary>
		public DbSet<DespesaAVista> DespesasAVista { get; set; }
		/// <summary>
		/// Contas A Pagar
		/// </summary>
		public DbSet<DespesaCartao> DespesasCartao { get; set; }
		/// <summary>
		/// Mês referente aos registros lançados
		/// </summary>
		public DbSet<Competencia> Competencias { get; set; }

		public FinanceiroDBContext(DbContextOptions<FinanceiroDBContext> options) : base(options)
		{
			ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
			ChangeTracker.AutoDetectChangesEnabled = false;
		}
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
				property.SetColumnType("varchar(100)");

			modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinanceiroDBContext).Assembly);
			modelBuilder.Ignore<ContasAPagar>();

			foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys())) relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;

			base.OnModelCreating(modelBuilder);
		}
	}
}
