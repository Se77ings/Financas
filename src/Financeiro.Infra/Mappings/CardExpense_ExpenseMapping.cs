using Financeiro.Business.Aggregates.Payable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Financeiro.Infra.Mappings
{
	public class CardExpense_ExpenseMapping : IEntityTypeConfiguration<CardExpense.Expense>
	{
		public void Configure(EntityTypeBuilder<CardExpense.Expense> builder)
		{
			builder.Property(x => x.CurrentInstallment).IsRequired();
			builder.Property(x => x.QuantityInstallments).IsRequired();
			builder.ToTable("CardExpense_Expense");
		}
	}
}
