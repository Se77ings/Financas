using Financeiro.Business.Aggregates.Payable;
using Financeiro.Business.Interfaces;
using Financeiro.Infra.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Financeiro.Infra.Repository
{
    public class RecurrentPaymentRepository : IRecurrentPaymentRepository
    {
        private readonly FinanceiroDBContext _context;

        public RecurrentPaymentRepository(FinanceiroDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RecurrentPayment entity)
        {
            _context.RecurrentPayment.Add(entity);
            await SaveChangesAsync();
        }

        public async Task<RecurrentPayment> GetByIdAsync(Guid id)
        {
            return await _context.RecurrentPayment
                .AsSplitQuery()
                .AsNoTracking()
                .Include(x => x.CreditCard)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<RecurrentPayment>> GetListAsync()
        {
            return await _context.RecurrentPayment
                .AsSplitQuery()
                .AsNoTracking()
                .Include(x => x.CreditCard)
                .ToListAsync();
        }

        public async Task<List<RecurrentPayment>> GetActiveRecurrentPaymentsAsync()
        {
            return await _context.RecurrentPayment
                .AsSplitQuery()
                .AsNoTracking()
                .Include(x => x.CreditCard)
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        public async Task<List<RecurrentPayment>> GetByCreditCardAsync(Guid creditCardId)
        {
            return await _context.RecurrentPayment
                .AsNoTracking()
                .Include(x => x.CreditCard)
                .Where(x => x.CreditCardId == creditCardId)
                .OrderBy(x => x.Description)
                .ToListAsync();
        }

        public async Task<List<RecurrentPayment>> GetWithoutCreditCardAsync()
        {
            return await _context.RecurrentPayment
                .AsNoTracking()
                .Where(x => x.CreditCardId == null)
                .OrderBy(x => x.Description)
                .ToListAsync();
        }

        // ExecuteUpdate grava direto no banco, sem anexar as assinaturas (e o cartão incluído nelas) ao contexto,
        // que entraria em conflito com o cartão salvo logo em seguida pelo mesmo serviço.
        public async Task DeactivateByCreditCardAsync(Guid creditCardId)
        {
            await _context.RecurrentPayment
                .Where(x => x.CreditCardId == creditCardId && x.IsActive)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsActive, false));
        }

        public async Task UpdateAsync(RecurrentPayment entity)
        {
            _context.RecurrentPayment.Update(entity);
            await SaveChangesAsync();
        }

        public async Task RemoveAsync(RecurrentPayment entity)
        {
            _context.RecurrentPayment.Remove(entity);
            await SaveChangesAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
