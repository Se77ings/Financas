using System;
using System.Threading.Tasks;
using Financeiro.Business.Shared;
using Financeiro.Business.Shared.ResultPattern;

namespace Financas.Application.Services.CompetenciaServices
{
	public interface IEffectiveDateGeneratorService
	{
		Task<Result<bool>> GenerateAutomaticExpensesAsync(Guid effectiveDateId, CalendarMonth month, int year);
		Task<Result<bool>> SyncMissingCardInvoicesAsync(Guid effectiveDateId);
	}
}
