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
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly FinanceiroDBContext _context;

        public SubscriptionRepository(FinanceiroDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Subscription entity)
        {
            _context.Subscription.Add(entity);
            await SaveChangesAsync();
        }

        public async Task<Subscription> GetByIdAsync(Guid id)
        {
            return await _context.Subscription
                .AsSplitQuery()
                .AsNoTracking()
                .Include(x => x.CreditCard)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Subscription>> GetListAsync()
        {
            return await _context.Subscription
                .AsSplitQuery()
                .AsNoTracking()
                .Include(x => x.CreditCard)
                .ToListAsync();
        }

        public async Task<List<Subscription>> GetActiveSubscriptionsAsync()
        {
            return await _context.Subscription
                .AsSplitQuery()
                .AsNoTracking()
                .Include(x => x.CreditCard)
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        public async Task<List<Subscription>> GetByCreditCardAsync(Guid creditCardId)
        {
            return await _context.Subscription
                .AsNoTracking()
                .Include(x => x.CreditCard)
                .Where(x => x.CreditCardId == creditCardId)
                .OrderBy(x => x.Description)
                .ToListAsync();
        }

        public async Task<List<Subscription>> GetWithoutCreditCardAsync()
        {
            return await _context.Subscription
                .AsNoTracking()
                .Where(x => x.CreditCardId == null)
                .OrderBy(x => x.Description)
                .ToListAsync();
        }

        // ExecuteUpdate grava direto no banco, sem anexar as assinaturas (e o cartão incluído nelas) ao contexto,
        // que entraria em conflito com o cartão salvo logo em seguida pelo mesmo serviço.
        public async Task DeactivateByCreditCardAsync(Guid creditCardId)
        {
            await _context.Subscription
                .Where(x => x.CreditCardId == creditCardId && x.IsActive)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsActive, false));
        }

        public async Task UpdateAsync(Subscription entity)
        {
            _context.Subscription.Update(entity);
            await SaveChangesAsync();
        }

        public async Task RemoveAsync(Subscription entity)
        {
            _context.Subscription.Remove(entity);
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
