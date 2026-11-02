using Financas.Application.Services.CompetenciaServices;
using Financas.Application.Services.PayableServices.Interfaces;
using Financas.Application.Services.SubscriptionServices;
using Financas.Business.Shared.ServiceBase;
using Financeiro.Application.Models.PayableModels;
using Financeiro.Application.Models.SubscriptionModels;
using Financeiro.Business.Aggregates.Payable;
using Financeiro.Business.Aggregates.Payable.Validations;
using Financeiro.Business.Interfaces;
using Financeiro.Business.Shared.ResultPattern;
using static Financeiro.Business.Aggregates.Payable.CardExpense;

namespace Financas.Application.Services.ObrigaçãoServices
{
    public class CardExpenseServices : ServiceBase, ICardExpenseServices
    {
        private readonly ICardExpenseRepository _repository;
        private readonly IEffectiveDateServices _effectiveDateServices;
        private readonly ISubscriptionServices _subscriptionServices;
        private readonly IEffectiveDateRepository _effectiveDateRepository;

        public CardExpenseServices(ICardExpenseRepository repository, IEffectiveDateServices effectiveDateServices, ISubscriptionServices subscriptionServices, IEffectiveDateRepository effectiveDateRepository)
        {
            _repository = repository;
            _effectiveDateServices = effectiveDateServices;
            _subscriptionServices = subscriptionServices;
            _effectiveDateRepository = effectiveDateRepository;
        }

        public async Task<Result<bool>> AddAsync(CardExpenseRequest request)
        {
            var explicitId = request.EffectiveDateId ?? request.EffectiveDate;
            var effectiveDateResult = await _effectiveDateServices.ResolveForOperationAsync(request.DueDate, explicitId);
            if (!effectiveDateResult.IsSuccess)
                return Result<bool>.Failure(effectiveDateResult.Error!);

            var effectiveDateId = effectiveDateResult.Value!;

            var entidade = new CardExpense(request.Description, request.SubTotal, request.DueDate, request.Discounts, request.Observation, effectiveDateId, request.CreditCardId, request.CategoryId);

            var validation = await ValidateEntityAsync<CardExpense, CardExpenseValidator>(entidade);

            if (!validation.Success)
                return Result<bool>.Failure(validation.Error);

            await _repository.AddAsync(entidade);
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> UpdateAsync(Guid id, CardExpenseRequest request)
        {
            var explicitId = request.EffectiveDateId ?? request.EffectiveDate;
            var effectiveDateResult = await _effectiveDateServices.ResolveForOperationAsync(request.DueDate, explicitId);
            if (!effectiveDateResult.IsSuccess)
                return Result<bool>.Failure(effectiveDateResult.Error!);

            var effectiveDateId = effectiveDateResult.Value!;

            var result = await GetCardExpenseById(id);

            var novaEntidade = new CardExpense(request.Description, request.SubTotal, request.DueDate, request.Discounts, request.Observation, effectiveDateId, request.CreditCardId, request.CategoryId);

            var validation = await ValidateEntityAsync<CardExpense, CardExpenseValidator>(novaEntidade);

            if (!validation.Success)
                return Result<bool>.Failure(validation.Error);

            result.Update(request.Description, request.SubTotal, request.DueDate, request.Discounts, request.Observation, effectiveDateId, request.CreditCardId, request.CategoryId);

            await _repository.UpdateAsync(result);
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            var result = await GetCardExpenseById(id);

            if (result.Expenses.Any())
                return Result<bool>.Failure(new Error("Não é possível excluir o cartão pois existem despesas vinculadas. Remova as despesas antes de prosseguir.", "500"));

            await _repository.RemoveAsync(result);
            return Result<bool>.Success(true);
        }

        public async Task<(Result<bool> result, List<CardExpenseView> lista)> GetListAsync()
        {
            var list = await _repository.GetListAsync();

            return (Result<bool>.Success(true), list.Select(x => (CardExpenseView)x).ToList());
        }

        public async Task<(Result<bool> result, CardExpenseView view)> GetByIdAsync(Guid id)
        {
            var despesaResult = await GetCardExpenseById(id);

            return (Result<bool>.Success(true), (CardExpenseView)despesaResult);
        }

        public async Task<Result<bool>> AddItemAsync(Guid id, CardExpenseItemRequest request)
        {
            var cardInvoice = await _repository.GetByIdAsync(id);
            if (cardInvoice == null)
                return Result<bool>.Failure(new Error("Fatura de cartão não encontrada."));

            var initialEffectiveDate = await _effectiveDateRepository.GetByIdAsync(cardInvoice.EffectiveDateId);
            if (initialEffectiveDate == null)
                return Result<bool>.Failure(new Error("Competência inicial não encontrada."));

            var affectedCardExpenseIds = new HashSet<Guid>();

            for (int i = 0; i < request.QuantityInstallments; i++)
            {
                var (targetMonth, targetYear) = initialEffectiveDate.GetNextPeriod(i);

                var targetEffectiveDate = await _effectiveDateRepository.GetByMonthYearAsync(targetMonth, targetYear);
                if (targetEffectiveDate == null)
                    break;

                var targetCardExpense = targetEffectiveDate.GetCardExpenseByCreditCardId(cardInvoice.CreditCardId);
                if (targetCardExpense == null)
                    break;

                int currentInstallment = i + 1;
                var expenseDueDate = request.DueDate.AddMonths(i);

                var expense = new Expense(
                    request.Description,
                    request.SubTotal,
                    expenseDueDate,
                    request.Discounts,
                    request.Observation,
                    currentInstallment,
                    request.QuantityInstallments,
                    targetEffectiveDate.Id,
                    targetCardExpense.Id,
                    request.CategoryId
                );

                await _repository.AddItemAsync(expense);
                affectedCardExpenseIds.Add(targetCardExpense.Id);
            }

            // Salva os itens antes da assinatura: o SaveChanges dela grava o contexto inteiro, e o
            // SaveChanges seguinte dos itens retornaria 0, acusando erro mesmo com tudo salvo.
            var savedChanges = await _repository.SaveChangesAsync();
            if (savedChanges == 0)
            {
                return Result<bool>.Failure(new Error("Erro ao salvar item de despesa."));
            }

            foreach (var cardExpenseId in affectedCardExpenseIds)
            {// TODO: Paleativo, resolver com o #FN-11.
                await _repository.RecalculateCardExpenseAsync(cardExpenseId);
            }

            if (request.isSubscription)
            {
                var subscriptionRequest = new SubscriptionRequest
                {
                    CreditCardId = cardInvoice.CreditCardId,
                    Description = request.Description,
                    EndDate = request.SubscriptionEndDate,
                    FrequencyType = request.SubscriptionFrequencyType,
                    IsActive = true,
                    StartDate = request.SubscriptionStartDate,
                    Value = request.SubTotal,
                    CategoryId = request.CategoryId
                };

                var result = await _subscriptionServices.AddAsync(subscriptionRequest);
                if (!result.IsSuccess)
                    return Result<bool>.Failure(new Error("O item foi salvo, mas houve erro ao criar a assinatura."));
            }

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> UpdateItemAsync(Guid id, Guid itemId, CardExpenseItemRequest request)
        {
            var result = await GetCardExpenseById(id);

            result.UpdateExpense(itemId, request.Description, request.SubTotal, request.DueDate, request.Discounts, request.Observation, request.CurrentInstallment, request.QuantityInstallments, request.CategoryId);

            await _repository.UpdateAsync(result);
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> RemoveItemAsync(Guid id, Guid itemId)
        {
            var result = await GetCardExpenseById(id);
            var expense = result.Expenses.FirstOrDefault(x => x.Id == itemId);

            if (expense != null)
            {
                result.RemoveExpense(itemId);
                await _repository.RemoveItemAsync(expense);
                await _repository.UpdateAsync(result);
            }

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> TransferItemsAsync(CardExpenseItemTransferRequest request)
        {
            if (request == null || request.CardExpenseItemIds == null || !request.CardExpenseItemIds.Any())
                return Result<bool>.Failure(new Error("Nenhum item selecionado para transferência.", "400"));

            var sourceCardExpenses = await _repository.GetCardExpensesByItemIdsAsync(request.CardExpenseItemIds);
            if (!sourceCardExpenses.Any())
                return Result<bool>.Failure(new Error("Fatura(s) de origem não encontrada(s).", "404"));

            var targetCompetenciaResult = await _effectiveDateServices.GetByIdAsync(request.TargetEffectiveDateId);
            if (!targetCompetenciaResult.result.IsSuccess || targetCompetenciaResult.view == null)
                return Result<bool>.Failure(new Error("Competência de destino não encontrada.", "404"));

            var targetCardExpensesCache = new Dictionary<Guid, CardExpense>();
            var updatedCardExpenses = new HashSet<CardExpense>();

            foreach (var itemId in request.CardExpenseItemIds)
            {
                var sourceCard = sourceCardExpenses.FirstOrDefault(c => c.Expenses.Any(e => e.Id == itemId));
                if (sourceCard == null) continue;

                var item = sourceCard.Expenses.First(e => e.Id == itemId);
                var creditCardId = sourceCard.CreditCardId;

                if (!targetCardExpensesCache.TryGetValue(creditCardId, out var targetCard))
                {
                    targetCard = await _repository.GetByCardAndEffectiveDateAsync(creditCardId, request.TargetEffectiveDateId);
                    if (targetCard == null)
                    {
                        int year = targetCompetenciaResult.view.Year;
                        int month = (int)targetCompetenciaResult.view.Month;
                        int day = Math.Min(sourceCard.DueDate.Day, DateTime.DaysInMonth(year, month));
                        DateOnly targetDueDate = new DateOnly(year, month, day);

                        targetCard = new CardExpense(
                            sourceCard.Description,
                            0,
                            targetDueDate,
                            0,
                            sourceCard.Observation,
                            request.TargetEffectiveDateId,
                            creditCardId,
                            sourceCard.CategoryId
                        );

                        await _repository.AddAsync(targetCard);
                    }
                    targetCardExpensesCache[creditCardId] = targetCard;
                }

                sourceCard.RemoveExpense(itemId);
                targetCard.AddExpense(item);
                item.TransferTo(targetCard.Id, request.TargetEffectiveDateId);
                await _repository.UpdateItemAsync(item);

                updatedCardExpenses.Add(sourceCard);
                updatedCardExpenses.Add(targetCard);
            }

            foreach (var card in updatedCardExpenses)
            {
                await _repository.UpdateAsync(card);
            }

            return Result<bool>.Success(true);
        }

        #region Métodos internos

        private async Task<CardExpense> GetCardExpenseById(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        #endregion
    }
}
