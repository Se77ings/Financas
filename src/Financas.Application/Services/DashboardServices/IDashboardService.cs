using Financeiro.Application.Models.DashboardModels;
using Financeiro.Business.Shared;
using Financeiro.Business.Shared.ResultPattern;

namespace Financas.Application.Services.DashboardServices
{
    public interface IDashboardService
    {
        /// <summary>Sem <paramref name="year"/>, considera todas as competências; com ele, o mês é obrigatório.</summary>
        Task<Result<DashboardView>> GetAsync(int? year, CalendarMonth? month, DateOnly today);
    }
}
