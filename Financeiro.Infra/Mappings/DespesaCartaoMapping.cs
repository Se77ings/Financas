using Financeiro.Business.Aggregates.Obrigação;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Financeiro.Infra.Mappings
{
	public class DespesaCartaoMapping : IEntityTypeConfiguration<DespesaCartao>
	{
		public void Configure(EntityTypeBuilder<DespesaCartao> builder)
		{
			builder.ToTable("DespesaCartao");
			builder.HasMany<DespesaCartao.Despesa>("Despesas").WithOne(x => x.DespesaCartao).HasForeignKey(x => x.DespesaCartaoId).OnDelete(DeleteBehavior.Restrict);
		}
	}
}
