using Financeiro.Business.Aggregates.EffectiveDate;
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
			builder.HasIndex(x => new { x.Month, x.Year }).IsUnique();
			builder.HasMany(x => x.Receivable).WithOne(x => x.EffectiveDate).HasForeignKey(x => x.EffectiveDateId);
			builder.HasMany(x => x.Payable).WithOne(x => x.EffectiveDate).HasForeignKey(x => x.EffectiveDateId);
		}
	}
}
