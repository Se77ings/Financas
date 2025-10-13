using Financeiro.Business.Aggregates.Obrigação;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Financeiro.Infra.Mappings
{
	public class DespesaAVistaMapping : IEntityTypeConfiguration<DespesaAVista>
	{
		public void Configure(EntityTypeBuilder<DespesaAVista> builder)
		{
			builder.ToTable("DespesaAVista");
		}
	}
}
