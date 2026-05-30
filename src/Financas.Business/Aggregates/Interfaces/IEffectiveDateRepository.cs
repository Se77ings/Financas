using Financeiro.Business.Shared;

namespace Financeiro.Business.Aggregates.Interfaces
{
	public interface IEffectiveDateRepository : IRepository<EffectiveDate>
	{
		Task<EffectiveDate?> GetByMonthYearAsync(CalendarMonth month, int year);
		Task<bool> ExistsAsync(CalendarMonth month, int year);
		Task<bool> ExistsOtherAsync(Guid id, CalendarMonth month, int year);
	}
}
