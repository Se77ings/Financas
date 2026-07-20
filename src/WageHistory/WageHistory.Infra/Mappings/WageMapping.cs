using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WageHistory.Business.Aggregates;

namespace WageHistory.Infra.Mappings
{
	public class WageMapping : IEntityTypeConfiguration<Wage>
	{
		public void Configure(EntityTypeBuilder<Wage> builder)
		{
			builder.HasKey(x => x.Id);

			builder.Property(x => x.GrossValue)
				.HasColumnType("decimal(18,2)")
				.IsRequired();

			builder.Property(x => x.NetValue)
				.HasColumnType("decimal(18,2)")
				.IsRequired();

			builder.Property(x => x.PaymentDate)
				.IsRequired();

			builder.OwnsMany(x => x.Deductions, d =>
			{
				d.WithOwner().HasForeignKey("WageId");
				d.Property<Guid>("Id");
				d.HasKey("Id");

				d.Property(x => x.Description)
					.HasColumnType("varchar(100)")
					.IsRequired();

				d.Property(x => x.Value)
					.HasColumnType("decimal(18,2)")
					.IsRequired();

				d.ToTable("WageDeductions");
			});

			builder.ToTable("Wages");
		}
	}
}
