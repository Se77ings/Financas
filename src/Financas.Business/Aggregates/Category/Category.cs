
using Financeiro.Business.Shared;

namespace Financeiro.Business.Aggregates.Category
{
    public class Category : Entity
    {
        public string Name { get; private set; }
        public string? Description { get; private set; }

        protected Category() { }

        public Category(string name, string? description)
        {
            Name = name;
            Description = NormalizeDescription(description);
        }

        public void Update(string name, string? description)
        {
            Name = name;
            Description = NormalizeDescription(description);
        }

        private static string? NormalizeDescription(string? description)
        {
            return string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        }
    }
}
