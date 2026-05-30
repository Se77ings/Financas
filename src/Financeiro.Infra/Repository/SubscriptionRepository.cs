using Financeiro.Business.Aggregates.Interfaces;
using Financeiro.Business.Aggregates.Payable;
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
				.Include(x => x.CreditCard)
				.FirstOrDefaultAsync(x => x.Id == id);
		}

		public async Task<List<Subscription>> GetListAsync()
		{
			return await _context.Subscription
				.Include(x => x.CreditCard)
				.ToListAsync();
		}

		public async Task<List<Subscription>> GetActiveSubscriptionsAsync()
		{
			return await _context.Subscription
				.Include(x => x.CreditCard)
				.Where(x => x.IsActive)
				.ToListAsync();
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
