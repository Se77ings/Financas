using Financeiro.Business.Aggregates.Payable;
using Financeiro.Business.Shared;

namespace Financas.Tests.Domain
{
    public class RecurrentPaymentTests
    {
        private static RecurrentPayment New(FrequencyType frequency, DateOnly start, DateOnly? end = null) =>
            new("Internet", 99.99m, frequency, null, start, end, null);

        [Theory]
        [InlineData(CalendarMonth.Jan, 2026, true)]
        [InlineData(CalendarMonth.Jun, 2026, true)]
        [InlineData(CalendarMonth.Dec, 2025, false)]
        public void Mensal_ValeDoMesDeInicioEmDiante(CalendarMonth month, int year, bool expected)
        {
            var payment = New(FrequencyType.Mensal, new DateOnly(2026, 1, 15));

            Assert.Equal(expected, payment.IsDueIn(month, year));
        }

        [Theory]
        [InlineData(CalendarMonth.Jan, true)]
        [InlineData(CalendarMonth.Feb, false)]
        [InlineData(CalendarMonth.Apr, true)]
        [InlineData(CalendarMonth.Jul, true)]
        public void Trimestral_ValeACadaTresMesesDesdeOInicio(CalendarMonth month, bool expected)
        {
            var payment = New(FrequencyType.Trimestral, new DateOnly(2026, 1, 10));

            Assert.Equal(expected, payment.IsDueIn(month, 2026));
        }

        [Fact]
        public void NaoValeDepoisDaDataDeFim()
        {
            var payment = New(FrequencyType.Mensal, new DateOnly(2026, 1, 10), end: new DateOnly(2026, 3, 31));

            Assert.True(payment.IsDueIn(CalendarMonth.Mar, 2026));
            Assert.False(payment.IsDueIn(CalendarMonth.Apr, 2026));
        }
    }
}
