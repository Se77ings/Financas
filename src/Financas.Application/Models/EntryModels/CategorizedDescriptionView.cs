namespace Financeiro.Application.Models.EntryModels
{
    public class CategorizedDescriptionView
    {
        public string Description { get; set; }
        public Guid CategoryId { get; set; }
        public int Count { get; set; }
    }
}
