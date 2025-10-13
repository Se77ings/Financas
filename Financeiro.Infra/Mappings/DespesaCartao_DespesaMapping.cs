using Financeiro.Business.Aggregates.Obrigação;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Financeiro.Infra.Mappings
{
	public class DespesaCartao_DespesaMapping : IEntityTypeConfiguration<DespesaCartao.Despesa>
	{
		public void Configure(EntityTypeBuilder<DespesaCartao.Despesa> builder)
		{
			builder.Property(x => x.ParcelaAtual).IsRequired();
			builder.Property(x => x.QtdParcelas).IsRequired();
			builder.ToTable("DespesaCartao_Cartao");
		}
	}
}
