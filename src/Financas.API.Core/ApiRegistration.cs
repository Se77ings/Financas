using Financas.API.Controllers;
using Financas.Application.Configuration;
using Financeiro.Infra.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Financas.API.Core
{
    public static class ApiRegistration
    {
        /// <summary>
        /// Devolve o <see cref="IMvcBuilder"/> para o host acrescentar os próprios controllers.
        /// </summary>
        public static IMvcBuilder AddFinanceiroApi(this IServiceCollection services, IConfiguration configuration)
        {
            var mvc = services.AddControllers().AddApplicationPart(typeof(ApiControllerBase).Assembly);

            services.RegisterServices();
            services.AddFinanceiroDatabase(configuration);

            return mvc;
        }
    }
}
