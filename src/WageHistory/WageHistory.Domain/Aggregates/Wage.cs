namespace WageHistory.Business.Aggregates
{
    public class Wage : Entity
    {
        public decimal GrossValue { get; private set; }
        public decimal NetValue { get; private set; }
        public DateOnly PaymentDate { get; private set; }

        private List<Deductions> _deductions = new List<Deductions>();
        public IEnumerable<Deductions> Deductions => _deductions.AsEnumerable();

        protected Wage() { }

        public Wage(decimal grossValue, decimal netValue, DateOnly paymentDate)
        {
            GrossValue = grossValue;
            NetValue = netValue;
            PaymentDate = paymentDate;
        }

        public void Update(decimal grossValue, DateOnly paymentDate)
        {
            GrossValue = grossValue;
            PaymentDate = paymentDate;
            RecalculateNetValue();
        }

        public void AddDeduction(string description, decimal value)
        {
            if (GrossValue - value < 0)
                throw new ArgumentException("Deduction value exceeds gross value");

            var deduction = new Deductions(description, value);
            _deductions.Add(deduction);

            NetValue -= value;
        }

        public void RemoveDeduction(string description, decimal value)
        {
            var deduction = _deductions.FirstOrDefault(d => d.Description == description && d.Value == value);

            if (deduction == null)
                throw new ArgumentException("Deduction not found");

            _deductions.Remove(deduction);
            NetValue += value;
        }

        public void ClearDeductions()
        {
            _deductions.Clear();
            NetValue = GrossValue;
        }

        private void RecalculateNetValue()
        {
            NetValue = GrossValue - _deductions.Sum(d => d.Value);
            if (NetValue < 0)
                throw new ArgumentException("Total deductions exceed gross value");
        }
    }
}
