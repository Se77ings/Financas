namespace Financeiro.Business.Interfaces
{
    /// <summary>
    /// Executa as gravações de vários repositórios numa única transação do banco.
    /// </summary>
    public interface ITransactionRunner
    {
        /// <summary>
        /// Confirma a transação quando <paramref name="operation"/> retorna <c>true</c>; desfaz tudo quando
        /// retorna <c>false</c> ou lança exceção. Dentro de uma transação já aberta, apenas executa a operação.
        /// </summary>
        Task<bool> RunAsync(Func<Task<bool>> operation);
    }
}
