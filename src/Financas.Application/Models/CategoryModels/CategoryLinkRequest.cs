namespace Financeiro.Application.Models.CategoryModels
{
    public class CategoryLinkRequest
    {
        public List<Guid> Ids { get; set; } = new();
        public Guid CategoryId { get; set; }
    }
}
