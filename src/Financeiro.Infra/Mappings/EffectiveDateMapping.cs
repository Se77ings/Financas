using Financeiro.Business.Aggregates;
using Financeiro.Business.Aggregates.Receivable;
using Financeiro.Business.Aggregates.Payable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Financeiro.Infra.Mappings
{
	public class EffectiveDateMapping : IEntityTypeConfiguration<EffectiveDate>
	{
		public void Configure(EntityTypeBuilder<EffectiveDate> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(x => x.Month).IsRequired().HasMaxLength(10);
			builder.Property(x => x.Year).IsRequired();
			builder.HasMany(x=> x.Receivable).WithOne(x=> x.EffectiveDate).HasForeignKey(x => x.EffectiveDateId).OnDelete(DeleteBehavior.Restrict);
			builder.HasMany(x=>x.Payable).WithOne(x => x.EffectiveDate).HasForeignKey(x => x.EffectiveDateId).OnDelete(DeleteBehavior.Restrict);
		}
	}
}
