namespace Financeiro.Application.Models.EntryModels
{
    public class EntriesByDescriptionView
    {
        public string Description { get; set; }
        public int Count { get; set; }
        public decimal Total { get; set; }
        public List<EntryView> Entries { get; set; } = new();
    }
}
