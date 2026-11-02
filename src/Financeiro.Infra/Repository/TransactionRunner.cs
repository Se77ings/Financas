using Financeiro.Business.Interfaces;
using Financeiro.Infra.Context;

namespace Financeiro.Infra.Repository
{
    public class TransactionRunner : ITransactionRunner
    {
        private readonly FinanceiroDBContext _context;

        public TransactionRunner(FinanceiroDBContext context)
        {
            _context = context;
        }

        public async Task<bool> RunAsync(Func<Task<bool>> operation)
        {
            if (_context.Database.CurrentTransaction != null)
                return await operation();

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (await operation())
                {
                    await transaction.CommitAsync();
                    return true;
                }

                await transaction.RollbackAsync();
                // Sem limpar, as entidades da tentativa desfeita continuariam no contexto e seriam
                // gravadas pelo próximo SaveChanges da mesma requisição.
                _context.ChangeTracker.Clear();
                return false;
            }
            catch
            {
                _context.ChangeTracker.Clear();
                throw;
            }
        }
    }
}
