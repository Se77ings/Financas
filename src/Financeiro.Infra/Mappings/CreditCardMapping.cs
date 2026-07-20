using Financeiro.Business.Aggregates.Payable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Financeiro.Infra.Mappings
{
	public class CreditCardMapping : IEntityTypeConfiguration<CreditCard>
	{
		public void Configure(EntityTypeBuilder<CreditCard> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
			builder.Property(x => x.ClosingDay).IsRequired();
			builder.Property(x => x.DueDay).IsRequired();
			builder.Property(x => x.Limit).IsRequired().HasPrecision(18, 2);
			builder.Property(x => x.IsActive).IsRequired();
            
			builder.HasMany(x => x.CardExpenses).WithOne(x => x.CreditCard).HasForeignKey(x => x.CreditCardId).OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("CreditCards");
		}
	}
}
