using System;
using System.Threading.Tasks;
using Financeiro.Business.Aggregates;
using Financeiro.Business.Shared;
using Financeiro.Business.Shared.ResultPattern;

namespace Financas.Application.Services.CompetenciaServices
{
	public interface IEffectiveDateGeneratorService
	{
		Task<Result<bool>> GenerateAutomaticExpensesAsync(EffectiveDate effectiveDate, CalendarMonth month, int year);
		Task<Result<bool>> SyncMissingCardInvoicesAsync(Guid effectiveDateId);
	}
}
