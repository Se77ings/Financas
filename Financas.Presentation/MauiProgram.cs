using Financas.Application.Configuration;
using Financeiro.Infra.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Financas.Presentation
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
#if DEBUG
            builder.Logging.AddDebug();
#endif

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "database.db");

            builder.Services.AddDbContext<FinanceiroDBContext>(options =>
            {
                options.UseSqlite($"Data Source={dbPath}");
            });
            builder.Services.RegisterServices();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<FinanceiroDBContext>();
                //SQLitePCL.raw.SetProvider();
                var foo = dbContext.Database.GetMigrations();
                
            }

            return app;
        }
    }
}
