using Financeiro.Business.Aggregates.EffectiveDate;
using Financeiro.Business.Aggregates.Payable;
using Financeiro.Business.Aggregates.Receivable;
using Financeiro.Business.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Financeiro.Infra.Mappings
{
	public class EffectiveDateMapping : IEntityTypeConfiguration<EffectiveDate>
	{
		public void Configure(EntityTypeBuilder<EffectiveDate> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(x => x.Month)
				.HasConversion<int>()
				.IsRequired();
			builder.Property(x => x.Year).IsRequired();
			builder.HasMany(x => x.Receivable).WithOne(x => x.EffectiveDate).HasForeignKey(x => x.EffectiveDateId).OnDelete(DeleteBehavior.Cascade);
			builder.HasMany(x => x.Payable).WithOne(x => x.EffectiveDate).HasForeignKey(x => x.EffectiveDateId).OnDelete(DeleteBehavior.Cascade);
		}
	}
}
