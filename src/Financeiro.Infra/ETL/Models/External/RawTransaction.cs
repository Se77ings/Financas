namespace Financeiro.Infra.ETL.Models.External
{
    public class RawTransaction
    {
        public string[] Values { get; set; }

        public RawTransaction(string[] values)
        {
            Values = values;
        }
    }
}
