namespace Financeiro.Business.Aggregates.Payable
{
    public class ImmediateExpense : Payable
    {

        protected ImmediateExpense() { }
        public ImmediateExpense(string description, decimal subtotal, DateOnly dueDate, decimal? discounts, string observation, Guid effectiveDateId, Guid? categoryId) : base(description, subtotal, dueDate, discounts, observation, effectiveDateId, categoryId)
        {

        }

        public override bool HasValue()
        {
            return this.Total > 0;
        }
    }
}
