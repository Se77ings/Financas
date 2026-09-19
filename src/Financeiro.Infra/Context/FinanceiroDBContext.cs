using Financeiro.Business.Aggregates.Receivable;
using Financeiro.Business.Aggregates.Payable;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
using Financeiro.Business.Aggregates.EffectiveDate;

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
        public DbSet<CreditCard> CreditCard { get; set; }
        public DbSet<Subscription> Subscription { get; set; }

        public FinanceiroDBContext(DbContextOptions<FinanceiroDBContext> options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.AutoDetectChangesEnabled = false;

            //SQLitePCL.Batteries_V2.Init();

            //this.Database.EnsureCreated();
        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    string dbPath = Path.Combine(FileSystem.CurrentDirectory, "database.db");

        //    optionsBuilder.UseSqlite($"Filename={dbPath}");
        //    //optionsBuilder.UseSqlServer("Server=Gabriel;Database=Financas;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true");

        //    base.OnConfiguring(optionsBuilder);
        //}

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
