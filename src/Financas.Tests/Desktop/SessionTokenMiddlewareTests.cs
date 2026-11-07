using Financas.Desktop.Hosting;
using Microsoft.AspNetCore.Http;

namespace Financas.Tests.Desktop
{
    public class SessionTokenMiddlewareTests
    {
        private const string Token = "token-da-sessao";

        private static async Task<(int Status, bool NextCalled)> InvokeAsync(string? cookie)
        {
            var nextCalled = false;
            var middleware = new SessionTokenMiddleware(_ =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            }, Token);

            var context = new DefaultHttpContext();
            if (cookie is not null) context.Request.Headers.Cookie = $"{SessionTokenMiddleware.CookieName}={cookie}";

            await middleware.InvokeAsync(context);

            return (context.Response.StatusCode, nextCalled);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("token-errado")]
        [InlineData("token-da-sessao-com-sufixo")]
        public async Task Recusa_sem_o_token_certo(string? cookie)
        {
            var (status, nextCalled) = await InvokeAsync(cookie);

            Assert.Equal(StatusCodes.Status401Unauthorized, status);
            Assert.False(nextCalled);
        }

        [Fact]
        public async Task Aceita_o_token_da_sessao()
        {
            var (status, nextCalled) = await InvokeAsync(Token);

            Assert.Equal(StatusCodes.Status200OK, status);
            Assert.True(nextCalled);
        }
    }
}
