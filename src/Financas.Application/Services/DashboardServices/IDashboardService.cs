using Financeiro.Application.Models.DashboardModels;
using Financeiro.Business.Shared.ResultPattern;

namespace Financas.Application.Services.DashboardServices
{
    public interface IDashboardService
    {
        Task<Result<DashboardView>> GetAsync(Guid effectiveDateId, DateOnly today);
    }
}
