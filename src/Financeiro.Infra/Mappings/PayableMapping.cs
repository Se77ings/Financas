using Financeiro.Business.Aggregates.Payable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Financeiro.Infra.Mappings
{
	public class PayableMapping : IEntityTypeConfiguration<Payable>
	{
		public void Configure(EntityTypeBuilder<Payable> builder)
		{
			builder.HasKey(x => x.Id);

			builder.Property(x => x.Total).IsRequired();
			builder.Property(x => x.DueDate).IsRequired();
			builder.Property(x => x.SubTotal).IsRequired();
			builder.Property(x => x.Observation).HasMaxLength(150);
		}
	}
}
