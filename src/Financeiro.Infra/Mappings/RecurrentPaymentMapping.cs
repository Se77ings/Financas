using Financeiro.Business.Aggregates.Payable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Financeiro.Infra.Mappings
{
	public class RecurrentPaymentMapping : IEntityTypeConfiguration<RecurrentPayment>
	{
		public void Configure(EntityTypeBuilder<RecurrentPayment> builder)
		{
			builder.HasKey(x => x.Id);
			builder.Property(x => x.Description).IsRequired().HasMaxLength(150);
			builder.Property(x => x.Value).IsRequired().HasPrecision(18, 2);
			builder.Property(x => x.FrequencyType).IsRequired().HasConversion<int>();
			builder.Property(x => x.StartDate).IsRequired();
			builder.Property(x => x.EndDate).IsRequired(false);
			builder.Property(x => x.IsActive).IsRequired();

			builder.HasOne(x => x.CreditCard)
				.WithMany()
				.HasForeignKey(x => x.CreditCardId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(x => x.Category)
				.WithMany()
				.HasForeignKey(x => x.CategoryId)
				.IsRequired(false);

			builder.ToTable("RecurrentPayments");
		}
	}
}
