using Financeiro.Business.Aggregates.EffectiveDate;
using Financeiro.Business.Shared;

namespace Financeiro.Business.Interfaces
{
    public interface IEffectiveDateRepository : IRepository<EffectiveDate>
    {
        Task<EffectiveDate?> GetByMonthYearAsync(CalendarMonth month, int year);
        Task<bool> ExistsAsync(CalendarMonth month, int year);
        Task<bool> ExistsOtherAsync(Guid id, CalendarMonth month, int year);
        Task RemoveAsync(Guid id);


    }
}
