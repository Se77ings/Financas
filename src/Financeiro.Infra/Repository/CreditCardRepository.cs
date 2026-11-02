using Financeiro.Business.Aggregates.Payable;
using Financeiro.Business.Interfaces;
using Financeiro.Infra.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Financeiro.Infra.Repository
{
	public class CreditCardRepository : ICreditCardRepository
	{
		private readonly FinanceiroDBContext _context;

		public CreditCardRepository(FinanceiroDBContext context)
		{
			_context = context;
		}

		public async Task AddAsync(CreditCard entity)
		{
			_context.CreditCard.Add(entity);
			await SaveChangesAsync();
		}

		public async Task<CreditCard> GetByIdAsync(Guid id)
		{
			return await _context.CreditCard
				.FirstOrDefaultAsync(x => x.Id == id);
		}

		public async Task<List<CreditCard>> GetListAsync()
		{
			return await _context.CreditCard.ToListAsync();
		}

		public async Task UpdateAsync(CreditCard entity)
		{
			_context.CreditCard.Update(entity);
			await SaveChangesAsync();
		}

		public async Task RemoveAsync(CreditCard entity)
		{
			_context.CreditCard.Remove(entity);
			await SaveChangesAsync();
		}

		public async Task<int> SaveChangesAsync()
		{
			return await _context.SaveChangesAsync();
		}

		public async Task<bool> ExistsAsync(string name)
		{
			return await _context.CreditCard.AnyAsync(x => x.Name.ToLower() == name.ToLower());
		}

		public async Task<bool> ExistsOtherAsync(Guid id, string name)
		{
			return await _context.CreditCard.AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.Id != id);
		}

		public async Task<int> CountInvoicesAsync(Guid creditCardId)
		{
			return await _context.Payable.OfType<CardExpense>().CountAsync(x => x.CreditCardId == creditCardId);
		}

		public async Task<int> CountRecurrentPaymentsAsync(Guid creditCardId)
		{
			return await _context.RecurrentPayment.CountAsync(x => x.CreditCardId == creditCardId);
		}

	}
}
