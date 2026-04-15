using Financeiro.Business.Aggregates.Payable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Financeiro.Infra.Mappings
{
	public class ImmediateExpenseMapping : IEntityTypeConfiguration<ImmediateExpense>
	{
		public void Configure(EntityTypeBuilder<ImmediateExpense> builder)
		{
			builder.ToTable("ImmediateExpense");
		}
	}
}
