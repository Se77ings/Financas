using Microsoft.Extensions.Logging;

namespace Financas.Desktop.Hosting
{
    /// <summary>
    /// O executável não tem console: sem isto, os erros do programa instalado não ficariam em lugar nenhum.
    /// </summary>
    public sealed class FileLoggerProvider : ILoggerProvider
    {
        private readonly string _folder;
        private readonly Lock _gate = new();

        public FileLoggerProvider(string folder)
        {
            _folder = folder;
        }

        public ILogger CreateLogger(string categoryName) => new FileLogger(this, categoryName);

        public void Dispose()
        {
        }

        private void Write(string line)
        {
            var path = Path.Combine(_folder, $"financeiro-{DateTime.Now:yyyy-MM-dd}.log");

            lock (_gate)
            {
                try
                {
                    File.AppendAllText(path, line + Environment.NewLine);
                }
                catch (IOException)
                {
                    // Sem log não é motivo para derrubar o programa.
                }
            }
        }

        private sealed class FileLogger : ILogger
        {
            private readonly FileLoggerProvider _provider;
            private readonly string _category;

            public FileLogger(FileLoggerProvider provider, string category)
            {
                _provider = provider;
                _category = category;
            }

            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

            public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                if (!IsEnabled(logLevel)) return;

                var line = $"{DateTimeOffset.Now:O} [{logLevel}] {_category}: {formatter(state, exception)}";
                _provider.Write(exception is null ? line : $"{line}{Environment.NewLine}{exception}");
            }
        }
    }
}
