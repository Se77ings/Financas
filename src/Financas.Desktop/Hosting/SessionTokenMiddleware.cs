using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Financas.Desktop.Hosting
{
    /// <summary>
    /// Qualquer programa do computador alcança a porta local; só a janela do Financeiro recebe o cookie com o token.
    /// </summary>
    public sealed class SessionTokenMiddleware
    {
        public const string CookieName = "financeiro_session";

        private readonly RequestDelegate _next;
        private readonly byte[] _token;

        public SessionTokenMiddleware(RequestDelegate next, string token)
        {
            _next = next;
            _token = Encoding.UTF8.GetBytes(token);
        }

        public Task InvokeAsync(HttpContext context)
        {
            var cookie = context.Request.Cookies[CookieName];

            if (cookie is null || !CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(cookie), _token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }

            return _next(context);
        }
    }
}
