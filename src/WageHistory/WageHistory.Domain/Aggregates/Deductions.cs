namespace WageHistory.Business.Aggregates
{
    public class Deductions
    {
        public string Description { get; private set; } = string.Empty;
        public decimal Value { get; private set; }
        protected Deductions() { }

        internal Deductions(string description, decimal value)
        {
            Description = description;
            Value = value;
        }
    }
}
