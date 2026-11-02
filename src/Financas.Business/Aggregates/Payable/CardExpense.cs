namespace Financeiro.Business.Aggregates.Payable
{
    public class CardExpense : Payable
    {
        private List<Expense> _expenses = new();
        public ICollection<Expense> Expenses => _expenses;

        public Guid CreditCardId { get; private set; }
        public CreditCard CreditCard { get; private set; }

        #region Construtores
        protected CardExpense() { }
        public CardExpense(string descricao, decimal subtotal, DateOnly dataVencimento, decimal? descontos, string observacao, Guid competencia, Guid creditCardId, Guid? categoryId) : base(descricao, subtotal, dataVencimento, descontos, observacao, competencia, categoryId)
        {
            CreditCardId = creditCardId;
        }
        #endregion

        public void Update(string description, decimal subtotal, DateOnly dueDate, decimal? discounts, string observation, Guid effectiveDateId, Guid creditCardId, Guid? categoryId)
        {
            base.Update(description, subtotal, dueDate, discounts, observation, effectiveDateId, categoryId);
            CreditCardId = creditCardId;
        }

        public void AddExpense(Expense expense)
        {
            _expenses.Add(expense);
            RecalculateValue();
        }
        public void RemoveExpense(Guid expenseId)
        {
            _expenses.Remove(_expenses.Find(x => x.Id == expenseId));
            RecalculateValue();
        }

        public void UpdateExpense(Guid expenseId, string description, decimal subtotal, DateOnly dueDate, decimal? discounts, string observation, int currentInstallment, int quantityInstallments, Guid? categoryId)
        {
            var expense = _expenses.Find(x => x.Id == expenseId);

            expense.UpdateExpense(description, subtotal, dueDate, discounts, observation, currentInstallment, quantityInstallments, categoryId);
            RecalculateValue();
        }

        private void RecalculateValue()
        {
            var listSubTotal = _expenses.Sum(x => x.SubTotal);
            var listDiscounts = _expenses.Sum(x => x.Discount);
            this.SetValues(listSubTotal, listDiscounts);
        }

        public override bool HasValue()
        {
            return _expenses.Any(x => x.Total > 0);
        }


        public class Expense : Payable
        {
            public int CurrentInstallment { get; private set; }
            public int QuantityInstallments { get; private set; }

            public Guid CardExpenseId { get; private set; }
            public CardExpense CardExpense { get; private set; }

            public Expense(string description, decimal subtotal, DateOnly dueDate, decimal? discounts, string observation, int currentInstallment, int quantityInstallments, Guid effectiveDate, Guid cardExpenseId, Guid? categoryId) : base(description, subtotal, dueDate, discounts, observation, effectiveDate, categoryId)
            {
                CurrentInstallment = currentInstallment;
                QuantityInstallments = quantityInstallments;
                CardExpenseId = cardExpenseId;
            }

            protected Expense() { }

            public void UpdateExpense(string descriptions, decimal subtotal, DateOnly dueDate, decimal? discounts, string observation, int currentInstallment, int quantityInstallments, Guid? categoryId)
            {
                CurrentInstallment = currentInstallment;
                QuantityInstallments = quantityInstallments;

                this.Update(descriptions, subtotal, dueDate, discounts, observation, EffectiveDateId, categoryId);
            }

            public void TransferTo(Guid cardExpenseId, Guid effectiveDateId)
            {
                CardExpenseId = cardExpenseId;
                this.Update(Description, SubTotal, DueDate, Discount, Observation, effectiveDateId, CategoryId);
            }

            public override bool HasValue()
            {
                return this.Total > 0;
            }
        }
    }

}
