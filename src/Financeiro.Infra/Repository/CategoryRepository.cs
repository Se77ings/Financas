using CategoryEntity = Financeiro.Business.Aggregates.Category.Category;
using Financeiro.Business.Interfaces;
using Financeiro.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace Financeiro.Infra.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly FinanceiroDBContext _context;

        public CategoryRepository(FinanceiroDBContext context)
        {
            _context = context;
        }
    
        public async Task AddAsync(CategoryEntity entity)
        {
            _context.Set<CategoryEntity>().Add(entity);
            await SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<bool> ExistsAsync(string name)
        {
            return await _context.Set<CategoryEntity>()
                .AnyAsync(x => x.Name == name);
        }

        public async Task<bool> ExistsOtherAsync(Guid id, string name)
        {
            return await _context.Set<CategoryEntity>()
                .AnyAsync(x => x.Name == name && x.Id != id);
        }

        public async Task<int> CountLinkedEntriesAsync(Guid id)
        {
            var payables = await _context.Payable.CountAsync(x => x.CategoryId == id);
            var receivables = await _context.Receivable.CountAsync(x => x.CategoryId == id);

            return payables + receivables;
        }

        public async Task<int> CountLinkedRecurrentPaymentsAsync(Guid id)
        {
            return await _context.RecurrentPayment.CountAsync(x => x.CategoryId == id);
        }

        public async Task<Dictionary<Guid, int>> CountLinkedEntriesByCategoryAsync()
        {
            var payables = await _context.Payable
                .Where(x => x.CategoryId != null)
                .GroupBy(x => x.CategoryId!.Value)
                .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                .ToListAsync();

            var receivables = await _context.Receivable
                .Where(x => x.CategoryId != null)
                .GroupBy(x => x.CategoryId!.Value)
                .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                .ToListAsync();

            return payables.Concat(receivables)
                .GroupBy(x => x.CategoryId)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Count));
        }

        public async Task<int> CountExistingPayablesAsync(IReadOnlyCollection<Guid> payableIds)
        {
            return await _context.Payable.CountAsync(x => payableIds.Contains(x.Id));
        }

        public async Task<int> CountExistingReceivablesAsync(IReadOnlyCollection<Guid> receivableIds)
        {
            return await _context.Receivable.CountAsync(x => receivableIds.Contains(x.Id));
        }

        // ExecuteUpdate grava direto no banco: atende qualquer tipo de Payable (TPT) sem carregar as entidades,
        // já que o contexto não rastreia alterações.
        public async Task SetPayablesCategoryAsync(IReadOnlyCollection<Guid> payableIds, Guid categoryId)
        {
            await _context.Payable
                .Where(x => payableIds.Contains(x.Id))
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.CategoryId, categoryId));
        }

        public async Task SetReceivablesCategoryAsync(IReadOnlyCollection<Guid> receivableIds, Guid categoryId)
        {
            await _context.Receivable
                .Where(x => receivableIds.Contains(x.Id))
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.CategoryId, categoryId));
        }

        public async Task<CategoryEntity> GetByIdAsync(Guid id)
        {
            return await _context.Set<CategoryEntity>()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<CategoryEntity>> GetListAsync()
        {
            return await _context.Set<CategoryEntity>().ToListAsync();
        }

        public async Task RemoveAsync(CategoryEntity entity)
        {
            _context.Set<CategoryEntity>().Remove(entity);
            await SaveChangesAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CategoryEntity entity)
        {
            _context.Set<CategoryEntity>().Update(entity);
            await SaveChangesAsync();
        }
    }
}
