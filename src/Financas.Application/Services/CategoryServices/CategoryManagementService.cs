using Financeiro.Application.Models.EntryModels;
using Financeiro.Business.Aggregates.Payable;
using Financeiro.Business.Aggregates.Receivable;
using Financeiro.Business.Interfaces;
using Financeiro.Business.Shared.ResultPattern;

namespace Financas.Application.Services.CategoryServices
{
    public class CategoryManagementService : ICategoryManagementService
    {
        private const int DescriptionMaxLength = 100;
        private const int SearchTermMinLength = 2;

        private readonly ICategoryEntriesRepository _repository;
        private readonly ICategoryRepository _categoryRepository;

        public CategoryManagementService(ICategoryEntriesRepository repository, ICategoryRepository categoryRepository)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
        }

        public async Task<Result<List<EntriesByDescriptionView>>> GetUncategorizedEntriesAsync()
        {
            return Result<List<EntriesByDescriptionView>>.Success(await GetEntriesGroupedByDescriptionAsync(null));
        }

        public async Task<Result<List<EntriesByDescriptionView>>> GetEntriesByCategoryAsync(Guid categoryId)
        {
            if (await _categoryRepository.GetByIdAsync(categoryId) is null)
                return Result<List<EntriesByDescriptionView>>.Failure(Error.NotFound("Categoria não encontrada."));

            return Result<List<EntriesByDescriptionView>>.Success(await GetEntriesGroupedByDescriptionAsync(categoryId));
        }

        public async Task<Result<bool>> RenameEntriesAsync(RenameEntriesRequest request)
        {
            var description = request.Description?.Trim();
            if (string.IsNullOrEmpty(description))
                return Result<bool>.Failure(new Error("A descrição não pode ser vazia."));

            if (description.Length > DescriptionMaxLength)
                return Result<bool>.Failure(new Error($"A descrição deve ter no máximo {DescriptionMaxLength} caracteres."));

            var payableIds = request.PayableIds.Distinct().ToList();
            var receivableIds = request.ReceivableIds.Distinct().ToList();

            if (payableIds.Count == 0 && receivableIds.Count == 0)
                return Result<bool>.Failure(new Error("Nenhum lançamento selecionado."));

            if (await _repository.CountExistingPayablesAsync(payableIds) != payableIds.Count
                || await _repository.CountExistingReceivablesAsync(receivableIds) != receivableIds.Count)
                return Result<bool>.Failure(Error.NotFound("Um ou mais lançamentos não foram encontrados."));

            if (payableIds.Count > 0)
                await _repository.SetPayablesDescriptionAsync(payableIds, description);

            if (receivableIds.Count > 0)
                await _repository.SetReceivablesDescriptionAsync(receivableIds, description);

            return Result<bool>.Success(true);
        }

        public async Task<Result<List<CategorizedDescriptionView>>> GetCategorizedDescriptionsAsync()
        {
            var descriptions = await _repository.GetCategorizedDescriptionsAsync();

            return Result<List<CategorizedDescriptionView>>.Success(descriptions
                .Select(x => new CategorizedDescriptionView { Description = x.Description, CategoryId = x.CategoryId, Count = x.Count })
                .ToList());
        }

        public async Task<Result<List<EntryView>>> SearchEntriesAsync(string? term)
        {
            var trimmed = term?.Trim();
            if (string.IsNullOrEmpty(trimmed) || trimmed.Length < SearchTermMinLength)
                return Result<List<EntryView>>.Failure(new Error($"Digite ao menos {SearchTermMinLength} caracteres para buscar."));

            var (immediateExpenses, cardExpenseItems, receivables) = await _repository.SearchByDescriptionAsync(trimmed);

            return Result<List<EntryView>>.Success(immediateExpenses.Select(ToEntryView)
                .Concat(cardExpenseItems.Select(ToEntryView))
                .Concat(receivables.Select(ToEntryView))
                .OrderByDescending(x => x.Date)
                .ThenBy(x => x.Description)
                .ToList());
        }

        private async Task<List<EntriesByDescriptionView>> GetEntriesGroupedByDescriptionAsync(Guid? categoryId)
        {
            var entries = new List<EntryView>();
            entries.AddRange((await _repository.GetImmediateExpensesByCategoryAsync(categoryId)).Select(ToEntryView));
            entries.AddRange((await _repository.GetCardExpenseItemsByCategoryAsync(categoryId)).Select(ToEntryView));
            entries.AddRange((await _repository.GetReceivablesByCategoryAsync(categoryId)).Select(ToEntryView));

            // Agrupa ignorando espaços nas pontas e maiúsculas, para que "Mercado" e "mercado " caiam juntos;
            // o grupo exibe a grafia mais usada.
            return entries
                .GroupBy(x => (x.Description ?? string.Empty).Trim().ToUpperInvariant())
                .Select(g => new EntriesByDescriptionView
                {
                    Description = g.GroupBy(x => x.Description).OrderByDescending(v => v.Count()).First().Key,
                    Count = g.Count(),
                    Total = g.Sum(x => x.Total),
                    Entries = g.OrderByDescending(x => x.Date).ToList()
                })
                .OrderByDescending(g => g.Count)
                .ThenBy(g => g.Description)
                .ToList();
        }

        private static EntryView ToEntryView(ImmediateExpense x) => new()
        {
            Id = x.Id,
            Type = EntryType.ImmediateExpense,
            Description = x.Description,
            Total = x.Total,
            Date = x.DueDate,
            Month = x.EffectiveDate.Month,
            Year = x.EffectiveDate.Year,
            Observation = x.Observation,
            CategoryId = x.CategoryId,
            CategoryName = x.Category?.Name
        };

        private static EntryView ToEntryView(CardExpense.Expense x) => new()
        {
            Id = x.Id,
            Type = EntryType.CardExpenseItem,
            Description = x.Description,
            Total = x.Total,
            Date = x.DueDate,
            Month = x.EffectiveDate.Month,
            Year = x.EffectiveDate.Year,
            CreditCardName = x.CardExpense?.CreditCard?.Name,
            CurrentInstallment = x.CurrentInstallment,
            QuantityInstallments = x.QuantityInstallments,
            Observation = x.Observation,
            CategoryId = x.CategoryId,
            CategoryName = x.Category?.Name
        };

        private static EntryView ToEntryView(Receivable x) => new()
        {
            Id = x.Id,
            Type = EntryType.Receivable,
            Description = x.Description,
            Total = x.Total,
            Date = x.Date,
            Month = x.EffectiveDate.Month,
            Year = x.EffectiveDate.Year,
            CategoryId = x.CategoryId,
            CategoryName = x.Category?.Name
        };
    }
}
