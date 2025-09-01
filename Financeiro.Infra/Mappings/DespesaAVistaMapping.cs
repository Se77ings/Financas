using Financeiro.Business.Aggregates.Obrigação;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
