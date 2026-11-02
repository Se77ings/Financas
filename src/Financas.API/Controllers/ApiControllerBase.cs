using Financeiro.Business.Shared.ResultPattern;
using Microsoft.AspNetCore.Mvc;

namespace Financas.API.Controllers
{
    /// <summary>
    /// Traduz os resultados dos services para HTTP de forma uniforme: sucesso → 200, registro inexistente → 404,
    /// demais falhas (regra de negócio, validação) → 400 com a mensagem em texto.
    /// </summary>
    public abstract class ApiControllerBase : Controller
    {
        /// <summary>Operação sem retorno: responde 200 sem corpo.</summary>
        protected IActionResult CommandResult(Result<bool> result)
        {
            return result.IsSuccess ? Ok() : FailureResult(result.Error!);
        }

        /// <summary>Operação que devolve dados: responde 200 com o valor.</summary>
        protected IActionResult QueryResult<T>(Result<T> result)
        {
            return result.IsSuccess ? Ok(result.Value) : FailureResult(result.Error!);
        }

        private IActionResult FailureResult(Error error)
        {
            return error.IsNotFound ? NotFound(error.Message) : BadRequest(error.Message);
        }
    }
}
