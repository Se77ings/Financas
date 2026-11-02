using Financeiro.Business.Aggregates.EffectiveDate;
using Financeiro.Business.Aggregates.Payable;
using Financeiro.Business.Interfaces;
using Financeiro.Business.Shared;
using Financeiro.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace Financeiro.Infra.Repository
{
    public class EffectiveDateRepository : IEffectiveDateRepository
    {
        private readonly FinanceiroDBContext _context;
        public EffectiveDateRepository(FinanceiroDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(EffectiveDate effectiveDate)
        {
            _context.EffectiveDate.Add(effectiveDate);
            await SaveChangesAsync();
        }
        public async Task UpdateAsync(EffectiveDate effectiveDate)
        {
            _context.EffectiveDate.Update(effectiveDate);
            await SaveChangesAsync();
        }

        public async Task RemoveAsync(EffectiveDate effectiveDate)
        {
            _context.EffectiveDate.Remove(effectiveDate);
            await SaveChangesAsync();
        }

        public async Task RemoveAsync(Guid id)
        {
            await _context.EffectiveDate.Where(x => x.Id == id).ExecuteDeleteAsync();

            await SaveChangesAsync();

        }

        public async Task<EffectiveDate> GetByIdAsync(Guid id)
        {
            return await _context.EffectiveDate
                .AsSplitQuery()
                .Include(x => x.Receivable)
                .Include(x => x.Payable)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        // Somente leitura: carrega os itens de cada fatura para a tela de detalhe sem rastreá-los,
        // para que o GetByIdAsync usado nas gravações continue sem arrastar os itens no Update.
        public async Task<EffectiveDate?> GetDetailsByIdAsync(Guid id)
        {
            return await _context.EffectiveDate
                .AsSplitQuery()
                .AsNoTracking()
                .Include(x => x.Receivable)
                .Include(x => x.Payable)
                    .ThenInclude((Payable p) => (p as CardExpense)!.Expenses)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<EffectiveDate>> GetListAsync()
        {
            // incluir coleções filhas para permitir cálculos e exibição de informações resumo
            return await _context.EffectiveDate
                .AsSplitQuery()
                .AsNoTracking()
                .Include(x => x.Receivable)
                .Include(x => x.Payable)
                .ToListAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }


        public async Task<EffectiveDate?> GetByMonthYearAsync(CalendarMonth month, int year)
        {
           return await _context.EffectiveDate
                .AsSplitQuery()
                .Include(x => x.Receivable)
                .Include(x => x.Payable)
                    .ThenInclude((Payable p) => (p as CardExpense)!.Expenses)
                .FirstOrDefaultAsync(x => x.Month == month && x.Year == year);
        }

        public async Task<bool> ExistsAsync(CalendarMonth month, int year)
        {
            return await _context.EffectiveDate.AnyAsync(x => x.Month == month && x.Year == year);
        }

        public async Task<bool> ExistsOtherAsync(Guid id, CalendarMonth month, int year)
        {
            return await _context.EffectiveDate.AnyAsync(x => x.Month == month && x.Year == year && x.Id != id);
        }
    }
}
