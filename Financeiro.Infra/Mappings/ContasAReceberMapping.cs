using Financeiro.Business.Aggregates.Direito;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Financeiro.Infra.Mappings
{
	public class ContasAReceberMapping : IEntityTypeConfiguration<ContasAReceber>
	{
		public void Configure(EntityTypeBuilder<ContasAReceber> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(x => x.Descricao).IsRequired();
			builder.Property(x => x.SubTotal).IsRequired();
			builder.Property(x => x.Descontos).IsRequired();
			builder.Property(x => x.Total).IsRequired();
			builder.Property(x => x.Data).IsRequired();
		}
	}
}
