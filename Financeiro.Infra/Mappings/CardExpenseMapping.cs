using Financeiro.Business.Aggregates.Payable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Financeiro.Infra.Mappings
{
	public class CardExpenseMapping : IEntityTypeConfiguration<CardExpense>
	{
		public void Configure(EntityTypeBuilder<CardExpense> builder)
		{
			builder.ToTable("CardExpenseMapping");
			builder.HasMany(x=> x.Expenses).WithOne(x => x.CardExpense).HasForeignKey(x => x.CardExpenseId).OnDelete(DeleteBehavior.Restrict);
		}
	}
}
