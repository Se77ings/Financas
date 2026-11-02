using Financeiro.Business.Aggregates.Payable;

namespace Financeiro.Business.Interfaces
{
	public interface ICardExpenseRepository : IRepository<CardExpense>
	{
		/// <summary>
		/// Registra, sem gravar, um item já incluído na fatura por <see cref="CardExpense.AddExpense"/>, junto com
		/// os totais que a fatura recalculou.
		/// <para>
		/// Não usa <c>Update(fatura)</c>: o contexto não rastreia consultas e o Id do item é gerado no construtor,
		/// então o EF percorreria o grafo e marcaria o item novo como alterado (UPDATE numa linha inexistente)
		/// em vez de novo. Por isso o item é adicionado explicitamente e só a fatura é marcada como alterada.
		/// </para>
		/// </summary>
		void AddItem(CardExpense invoice, CardExpense.Expense item);
		Task RemoveItemAsync(CardExpense.Expense item);
		Task<bool> ExistsAsync(string description, DateOnly date);
		Task<bool> ExistsOtherAsync(Guid id, string description, DateOnly date);

		Task<bool> ExistsItemAsync(string description, DateOnly date);
		Task<bool> ExistsOtherItemAsync(Guid id, string description, DateOnly date);
		Task<CardExpense?> GetByCardAndEffectiveDateAsync(Guid creditCardId, Guid effectiveDateId);
		Task<List<CardExpense>> GetCardExpensesByItemIdsAsync(List<Guid> itemIds);
		Task<CardExpense?> GetLatestByCreditCardAsync(Guid creditCardId);
		Task UpdateItemAsync(CardExpense.Expense item);
	}
}
