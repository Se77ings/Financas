using Financeiro.Business.Aggregates.Receivable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Financeiro.Infra.Mappings
{
	public class ReceivableMapping : IEntityTypeConfiguration<Receivable>
	{
		public void Configure(EntityTypeBuilder<Receivable> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(x => x.Description).IsRequired();
			builder.Property(x => x.SubTotal).IsRequired().HasPrecision(18, 2);
			builder.Property(x => x.Discounts).IsRequired().HasPrecision(18, 2);
			builder.Property(x => x.Total).IsRequired().HasPrecision(18, 2);
			builder.Property(x => x.Date).IsRequired();
		}
	}
}
