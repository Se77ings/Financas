using Financeiro.Business.Aggregates.Obrigação;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Financeiro.Infra.Mappings
{
	public class ContasAPagarMapping : IEntityTypeConfiguration<ContasAPagar>
	{
		public void Configure(EntityTypeBuilder<ContasAPagar> builder)
		{
			builder.HasKey(x => x.Id);

			builder.Property(x => x.Total).IsRequired();
			builder.Property(x => x.DataVencimento).IsRequired();
			builder.Property(x => x.SubTotal).IsRequired();
			builder.Property(x => x.Observacao).HasMaxLength(150);
		}
	}
}
