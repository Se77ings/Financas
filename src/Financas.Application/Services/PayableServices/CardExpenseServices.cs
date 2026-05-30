using Financas.Application.Services.CompetenciaServices;
using Financas.Application.Services.PayableServices.Interfaces;
using Financas.Business.Shared.ServiceBase;
using Financeiro.Application.Models.PayableModels;
using Financeiro.Business.Aggregates.Interfaces;
using Financeiro.Business.Aggregates.Payable;
using Financeiro.Business.Aggregates.Validações;
using Financeiro.Business.Shared.ResultPattern;
using static Financeiro.Business.Aggregates.Payable.CardExpense;

namespace Financas.Application.Services.ObrigaçãoServices
{
    public class CardExpenseServices : ServiceBase, ICardExpenseServices
    {
        private readonly ICardExpenseRepository _repository;
        private readonly IEffectiveDateServices _effectiveDateServices;

        public CardExpenseServices(ICardExpenseRepository repository, IEffectiveDateServices effectiveDateServices)
        {
            _repository = repository;
            _effectiveDateServices = effectiveDateServices;
        }

        public async Task<Result<bool>> AddAsync(CardExpenseRequest request)
        {
            var explicitId = request.EffectiveDateId ?? request.EffectiveDate;
            var effectiveDateResult = await _effectiveDateServices.ResolveForOperationAsync(request.DueDate, explicitId);
            if (!effectiveDateResult.IsSuccess)
                return Result<bool>.Failure(effectiveDateResult.Error!);

            var effectiveDateId = effectiveDateResult.Value!;

            var entidade = new CardExpense(request.Description, request.SubTotal, request.DueDate, request.Discounts, request.Observation, effectiveDateId, request.CreditCardId);

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

            var novaEntidade = new CardExpense(request.Description, request.SubTotal, request.DueDate, request.Discounts, request.Observation, effectiveDateId, request.CreditCardId);

            var validation = await ValidateEntityAsync<CardExpense, CardExpenseValidator>(novaEntidade);

            if (!validation.Success)
                return Result<bool>.Failure(validation.Error);

            result.Update(request.Description, request.SubTotal, request.DueDate, request.Discounts, request.Observation, effectiveDateId, request.CreditCardId);

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
            var cartao = await _repository.GetByIdAsync(id);
            
            var despesa = new Expense(request.Description, request.SubTotal, request.DueDate, request.Discounts, request.Observation, request.CurrentInstallment, request.QuantityInstallments, cartao.EffectiveDateId, id);

            await _repository.AddItemAsync(despesa);
            await _repository.SaveChangesAsync();
            await _repository.RecalculateCardExpenseAsync(id);

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> UpdateItemAsync(Guid id, Guid itemId, CardExpenseItemRequest request)
        {
            var result = await GetCardExpenseById(id);

            result.UpdateExpense(itemId, request.Description, request.SubTotal, request.DueDate, request.Discounts, request.Observation, request.CurrentInstallment, request.QuantityInstallments);

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
