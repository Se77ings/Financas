using Financeiro.Business.Aggregates;
using Financeiro.Business.Aggregates.Receivable;
using Financeiro.Business.Aggregates.Payable;
using Microsoft.EntityFrameworkCore;

namespace Financeiro.Infra.Context
{
	public class FinanceiroDBContext : DbContext
	{
		/// <summary>
		/// Contas a Receber
		/// </summary>
		public DbSet<Receivable> Receivable { get; set; }
		/// <summary>
		/// Contas A Pagar
		/// </summary>
		public DbSet<Payable> Payable { get; set; }

		/// <summary>
		/// Mês/Ano referente aos registros lançados
		/// </summary>
		/// 
		public DbSet<EffectiveDate> EffectiveDate { get; set; }
		public DbSet<CardExpense.Expense> CardExpenseItem { get; set; }

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

			foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys())) relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;

			base.OnModelCreating(modelBuilder);
		}
	}
}
