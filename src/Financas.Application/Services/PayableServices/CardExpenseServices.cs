using Financas.Application.Services.EffectiveDateServices;
using Financas.Application.Services.PayableServices.Interfaces;
using Financas.Application.Services.RecurrentPaymentServices;
using Financas.Business.Shared.ServiceBase;
using Financeiro.Application.Models.PayableModels;
using Financeiro.Application.Models.RecurrentPaymentModels;
using Financeiro.Business.Aggregates.Payable;
using Financeiro.Business.Aggregates.Payable.Validations;
using Financeiro.Business.Interfaces;
using Financeiro.Business.Shared;
using Financeiro.Business.Shared.ResultPattern;
using static Financeiro.Business.Aggregates.Payable.CardExpense;

namespace Financas.Application.Services.PayableServices
{
    public class CardExpenseServices : ServiceBase, ICardExpenseServices
    {
        private readonly ICardExpenseRepository _repository;
        private readonly IEffectiveDateServices _effectiveDateServices;
        private readonly IRecurrentPaymentServices _recurrentPaymentServices;
        private readonly IEffectiveDateRepository _effectiveDateRepository;
        private readonly ICreditCardRepository _creditCardRepository;
        private readonly ITransactionRunner _transactionRunner;

        public CardExpenseServices(ICardExpenseRepository repository, IEffectiveDateServices effectiveDateServices, IRecurrentPaymentServices recurrentPaymentServices, IEffectiveDateRepository effectiveDateRepository, ICreditCardRepository creditCardRepository, ITransactionRunner transactionRunner)
        {
            _transactionRunner = transactionRunner;
            _repository = repository;
            _effectiveDateServices = effectiveDateServices;
            _recurrentPaymentServices = recurrentPaymentServices;
            _effectiveDateRepository = effectiveDateRepository;
            _creditCardRepository = creditCardRepository;
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

            var cardInvoice = await GetCardExpenseById(id);
            if (cardInvoice == null)
                return Result<bool>.Failure(Error.NotFound("Fatura de cartão não encontrada."));

            cardInvoice.Update(request.Description, request.SubTotal, request.DueDate, request.Discounts, request.Observation, effectiveDateId, request.CreditCardId, request.CategoryId);

            var validation = await ValidateEntityAsync<CardExpense, CardExpenseValidator>(cardInvoice);

            if (!validation.Success)
                return Result<bool>.Failure(validation.Error);

            await _repository.UpdateAsync(cardInvoice);
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            var result = await GetCardExpenseById(id);
            if (result == null)
                return Result<bool>.Failure(Error.NotFound("Fatura de cartão não encontrada."));

            if (result.Expenses.Any())
                return Result<bool>.Failure(new Error("Não é possível excluir o cartão pois existem despesas vinculadas. Remova as despesas antes de prosseguir."));

            await _repository.RemoveAsync(result);
            return Result<bool>.Success(true);
        }

        public async Task<Result<List<CardExpenseView>>> GetListAsync()
        {
            var list = await _repository.GetListAsync();

            return Result<List<CardExpenseView>>.Success(list.Select(x => (CardExpenseView)x).ToList());
        }

        public async Task<Result<CardExpenseView>> GetByIdAsync(Guid id)
        {
            var cardInvoice = await GetCardExpenseById(id);
            if (cardInvoice == null)
                return Result<CardExpenseView>.Failure(Error.NotFound("Fatura de cartão não encontrada."));

            return Result<CardExpenseView>.Success((CardExpenseView)cardInvoice);
        }

        public async Task<Result<bool>> AddItemAsync(Guid id, CardExpenseItemRequest request)
        {
            var cardInvoice = await _repository.GetByIdAsync(id);
            if (cardInvoice == null)
                return Result<bool>.Failure(Error.NotFound("Fatura de cartão não encontrada."));

            var initialEffectiveDate = await _effectiveDateRepository.GetByIdAsync(cardInvoice.EffectiveDateId);
            if (initialEffectiveDate == null)
                return Result<bool>.Failure(Error.NotFound("Competência inicial não encontrada."));

            var requestedItem = new Expense(request.Description, request.SubTotal, request.DueDate, request.Discounts, request.Observation, request.CurrentInstallment, request.QuantityInstallments, cardInvoice.EffectiveDateId, cardInvoice.Id, request.CategoryId);
            var itemValidation = await ValidateEntityAsync<Expense, CardExpenseItemValidator>(requestedItem);
            if (!itemValidation.Success)
                return Result<bool>.Failure(itemValidation.Error);

            RecurrentPaymentRequest? recurrentPaymentRequest = null;
            if (request.IsRecurrent)
            {
                recurrentPaymentRequest = new RecurrentPaymentRequest
                {
                    CreditCardId = cardInvoice.CreditCardId,
                    Description = request.Description,
                    EndDate = request.RecurrenceEndDate,
                    FrequencyType = request.RecurrenceFrequencyType,
                    IsActive = true,
                    StartDate = request.RecurrenceStartDate,
                    Value = request.SubTotal,
                    CategoryId = request.CategoryId
                };

                // A assinatura só é gravada depois dos itens; validá-la aqui evita salvar itens de uma assinatura inválida.
                var recurrentPayment = new RecurrentPayment(recurrentPaymentRequest.Description, recurrentPaymentRequest.Value, recurrentPaymentRequest.FrequencyType, recurrentPaymentRequest.CreditCardId, recurrentPaymentRequest.StartDate, recurrentPaymentRequest.EndDate, recurrentPaymentRequest.CategoryId, recurrentPaymentRequest.IsActive);
                var recurrentPaymentValidation = await ValidateEntityAsync<RecurrentPayment, RecurrentPaymentValidator>(recurrentPayment);
                if (!recurrentPaymentValidation.Success)
                    return Result<bool>.Failure(recurrentPaymentValidation.Error);
            }

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

                targetCardExpense.AddExpense(expense);
                _repository.AddItem(targetCardExpense, expense);
            }

            // Itens e assinatura são gravados juntos. Os itens vão primeiro porque o SaveChanges da assinatura
            // grava o contexto inteiro, e o dos itens, chamado depois, retornaria 0.
            var outcome = Result<bool>.Success(true);
            await _transactionRunner.RunAsync(async () =>
            {
                if (await _repository.SaveChangesAsync() == 0)
                {
                    outcome = Result<bool>.Failure(new Error("Erro ao salvar item de despesa."));
                    return false;
                }

                if (recurrentPaymentRequest != null)
                {
                    var recurrentPaymentResult = await _recurrentPaymentServices.AddAsync(recurrentPaymentRequest);
                    if (!recurrentPaymentResult.IsSuccess)
                    {
                        outcome = Result<bool>.Failure(new Error("Erro ao criar a assinatura. O item não foi salvo."));
                        return false;
                    }
                }

                return true;
            });

            return outcome;
        }

        public async Task<Result<bool>> UpdateItemAsync(Guid id, Guid itemId, CardExpenseItemRequest request)
        {
            var cardInvoice = await GetCardExpenseById(id);
            if (cardInvoice == null)
                return Result<bool>.Failure(Error.NotFound("Fatura de cartão não encontrada."));

            var item = cardInvoice.Expenses.FirstOrDefault(x => x.Id == itemId);
            if (item == null)
                return Result<bool>.Failure(Error.NotFound("Item da fatura não encontrado."));

            cardInvoice.UpdateExpense(itemId, request.Description, request.SubTotal, request.DueDate, request.Discounts, request.Observation, request.CurrentInstallment, request.QuantityInstallments, request.CategoryId);

            var validation = await ValidateEntityAsync<Expense, CardExpenseItemValidator>(item);
            if (!validation.Success)
                return Result<bool>.Failure(validation.Error);

            await _repository.UpdateAsync(cardInvoice);
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> RemoveItemAsync(Guid id, Guid itemId)
        {
            var cardInvoice = await GetCardExpenseById(id);
            if (cardInvoice == null)
                return Result<bool>.Failure(Error.NotFound("Fatura de cartão não encontrada."));

            var expense = cardInvoice.Expenses.FirstOrDefault(x => x.Id == itemId);

            if (expense != null)
            {
                cardInvoice.RemoveExpense(itemId);
                await _repository.RemoveItemAsync(expense);
                await _repository.UpdateAsync(cardInvoice);
            }

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> TransferItemsAsync(CardExpenseItemTransferRequest request)
        {
            if (request == null || request.CardExpenseItemIds == null || !request.CardExpenseItemIds.Any())
                return Result<bool>.Failure(new Error("Nenhum item selecionado para transferência."));

            var sourceCardExpenses = await _repository.GetCardExpensesByItemIdsAsync(request.CardExpenseItemIds);
            if (!sourceCardExpenses.Any())
                return Result<bool>.Failure(Error.NotFound("Fatura(s) de origem não encontrada(s)."));

            var targetEffectiveDate = await _effectiveDateRepository.GetByIdAsync(request.TargetEffectiveDateId);
            if (targetEffectiveDate == null)
                return Result<bool>.Failure(Error.NotFound("Competência de destino não encontrada."));

            var outcome = Result<bool>.Success(true);
            await _transactionRunner.RunAsync(async () =>
            {
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
                            var creditCard = await _creditCardRepository.GetByIdAsync(creditCardId);
                            if (creditCard == null)
                            {
                                outcome = Result<bool>.Failure(Error.NotFound("Cartão de crédito não encontrado."));
                                return false;
                            }

                            targetCard = CreateInvoice(creditCard, targetEffectiveDate.Id, targetEffectiveDate.Month, targetEffectiveDate.Year);
                            await _repository.AddAsync(targetCard);
                        }
                        targetCardExpensesCache[creditCardId] = targetCard;
                    }

                    sourceCard.RemoveExpense(itemId);
                    targetCard.AddExpense(item);
                    item.TransferTo(targetCard, request.TargetEffectiveDateId);
                    await _repository.UpdateItemAsync(item);

                    updatedCardExpenses.Add(sourceCard);
                    updatedCardExpenses.Add(targetCard);
                }

                foreach (var card in updatedCardExpenses)
                {
                    await _repository.UpdateAsync(card);
                }

                return true;
            });

            return outcome;
        }

        #region Métodos internos

        private async Task<CardExpense?> GetCardExpenseById(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        // Mesma descrição e vencimento das faturas criadas pelo gerador de competências.
        private static CardExpense CreateInvoice(CreditCard card, Guid effectiveDateId, CalendarMonth month, int year)
        {
            var invoiceDescription = $"Fatura {card.Name} - {CalendarMonthHelper.ToPortugueseLabel(month, year)}";
            var dueDay = Math.Min(card.DueDay, DateTime.DaysInMonth(year, (int)month));
            var dueDate = new DateOnly(year, (int)month, dueDay);

            return new CardExpense(invoiceDescription, 0, dueDate, 0, null, effectiveDateId, card.Id, null);
        }

        #endregion
    }
}
