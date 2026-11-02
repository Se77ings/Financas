using Financas.Business.Shared.ServiceBase;
using Financeiro.Application.Models.EffectiveDateModels;
using Financeiro.Business.Shared;
using Financeiro.Business.Shared.ResultPattern;
using static Financeiro.Business.Shared.Localization.ErrorMessages;
using Financeiro.Business.Interfaces;
using Financeiro.Business.Aggregates.EffectiveDate;
using Financeiro.Business.Aggregates.EffectiveDate.Validations;

namespace Financas.Application.Services.CompetenciaServices
{
    public class EffectiveDateServices : ServiceBase, IEffectiveDateServices
    {
        private readonly IEffectiveDateRepository _repository;
        private readonly IEffectiveDateGeneratorService _generatorService;
        private readonly ITransactionRunner _transactionRunner;

        public EffectiveDateServices(IEffectiveDateRepository repository, IEffectiveDateGeneratorService generatorService, ITransactionRunner transactionRunner)
        {
            _repository = repository;
            _generatorService = generatorService;
            _transactionRunner = transactionRunner;
        }

        public async Task<Result<bool>> AddAsync(EffectiveDateRequest request)
        {
            if (await _repository.ExistsAsync(request.Month, request.Year))
            {
                return Result<bool>.Failure(new Error("Já existe uma competência para esse mês/ano."));
            }

            var creation = await CreateWithGeneratedEntriesAsync(request.Month, request.Year);
            return creation.IsSuccess
                ? Result<bool>.Success(true)
                : Result<bool>.Failure(creation.Error!);
        }

        public async Task<Result<bool>> EnsureExistsAsync(EffectiveDateRequest request)
        {
            if (await _repository.ExistsAsync(request.Month, request.Year))
                return Result<bool>.Success(false);

            var createResult = await GetOrCreateIdAsync(request.Month, request.Year);
            if (!createResult.IsSuccess)
                return Result<bool>.Failure(createResult.Error!);

            return Result<bool>.Success(true);
        }

        public async Task<Result<Guid>> GetOrCreateIdAsync(CalendarMonth month, int year)
        {
            var existing = await _repository.GetByMonthYearAsync(month, year);
            if (existing != null)
                return Result<Guid>.Success(existing.Id);

            return await CreateWithGeneratedEntriesAsync(month, year);
        }

        // A competência, as faturas e as recorrências geradas para ela são gravadas juntas: se a geração
        // falhar, a competência não fica salva sem os lançamentos automáticos.
        private async Task<Result<Guid>> CreateWithGeneratedEntriesAsync(CalendarMonth month, int year)
        {
            var effectiveDate = new EffectiveDate(month, year);

            var validation = await ValidateEntityAsync<EffectiveDate, EffectiveDateValidator>(effectiveDate);
            if (!validation.Success)
                return Result<Guid>.Failure(validation.Error);

            Result<bool> generation = Result<bool>.Success(true);
            await _transactionRunner.RunAsync(async () =>
            {
                await _repository.AddAsync(effectiveDate);
                generation = await _generatorService.GenerateAutomaticExpensesAsync(effectiveDate, month, year);
                return generation.IsSuccess;
            });

            return generation.IsSuccess
                ? Result<Guid>.Success(effectiveDate.Id)
                : Result<Guid>.Failure(generation.Error);
        }

        public async Task<Result<Guid>> ResolveForOperationAsync(DateOnly operationDate, Guid? explicitEffectiveDateId = null)
        {
            if (explicitEffectiveDateId is Guid id && id != Guid.Empty)
            {
                var effectiveDate = await GetEffectiveDate(id);
                if (effectiveDate == null)
                    return Result<Guid>.Failure(new Error(RecordNotFoundFormat(nameof(EffectiveDate))));

                return Result<Guid>.Success(id);
            }

            var month = CalendarMonthHelper.FromDateTime(operationDate.ToDateTime(TimeOnly.MinValue));
            return await GetOrCreateIdAsync(month, operationDate.Year);
        }

        public async Task<Result<bool>> UpdateAsync(Guid id, EffectiveDateRequest request)
        {

            var effectiveDate = await GetEffectiveDate(id);
            //TODO: Verificar se já existe a nova entidade ou homônima.

            if (effectiveDate == null)
                return Result<bool>.Failure(new Error(RecordNotFoundFormat(nameof(EffectiveDate))));

            var validation = await ValidateEntityAsync<EffectiveDate, EffectiveDateValidator>(effectiveDate);

            if (!validation.Success)
                return Result<bool>.Failure(validation.Error);

            effectiveDate.Update(request.Month, request.Year);

            await _repository.UpdateAsync(effectiveDate);

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            var effectiveDate = await GetEffectiveDate(id);

            if (effectiveDate == null)
                return Result<bool>.Failure(new Error(RecordNotFoundFormat(nameof(EffectiveDate))));

            // Impedir remoção se existirem valores/itens filhos
            if ((effectiveDate.Receivable != null && effectiveDate.Receivable.Any()) || (effectiveDate.Payable != null && effectiveDate.Payable.Any(x=> x.HasValue())))
            {
                return Result<bool>.Failure(new Error("Não é possível remover a competência enquanto existirem contas vinculadas (recebíveis ou pagáveis)."));
            }

            await _repository.RemoveAsync(id);

            return Result<bool>.Success(true);
        }

        public async Task<(Result<bool> result, EffectiveDateView view)> GetByIdAsync(Guid id)
        {
            var effectiveDate = await GetEffectiveDate(id);

            if(effectiveDate == null)
            {
                return (Result<bool>.Failure(new Error("Não há registros.")), null);
            }

            return (Result<bool>.Success(true), (EffectiveDateView)effectiveDate);
        }

        public async Task<(Result<bool> result, List<EffectiveDateSummary> lista)> GetListAsync()
        {

            var list = await _repository.GetListAsync();
            if (list == null)
                return (Result<bool>.Failure(new Error("Nenhuma competência encontrada.")), null);

            // Mapear para summary com informações úteis para exibição
            var summaries = list.Select(x => new EffectiveDateSummary
            {
                Id = x.Id,
                Month = x.Month,
                Year = x.Year,
                ReceivableCount = x.Receivable?.Count() ?? 0,
                PayableCount = x.Payable?.Count() ?? 0,
                Balance = (x.Receivable?.Sum(r => r.Total) ?? 0) - (x.Payable?.Sum(p => p.Total) ?? 0)
            }).OrderBy(x => x.Year)
              .ThenBy(x => x.Month)
              .ToList();

            return (Result<bool>.Success(true), summaries);
        }

        public async Task<Result<bool>> SyncMissingCardInvoicesAsync(Guid id)
        {
            var effectiveDate = await GetEffectiveDate(id);
            if (effectiveDate == null)
                return Result<bool>.Failure(new Error("Competência não encontrada."));

            return await _generatorService.SyncMissingCardInvoicesAsync(id);
        }

        public async Task<(Result<bool> result, List<EffectiveDateSummary> lista)> GetEligibleForTransferAsync()
        {
            //TODO: Atualmente todas as competências cadastradas são elegíveis. (futuramente alterar, quando houver opção de "quitar" as competencias
            return await GetListAsync();
        }

        #region Métodos internos
        public async Task<EffectiveDate?> GetEffectiveDate(Guid id)
        {
            return await _repository.GetByIdAsync(id);

        }

        #endregion
    }
}
