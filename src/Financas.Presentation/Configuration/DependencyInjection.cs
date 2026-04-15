using Financas.Application.Configuration;
using Financeiro.Infra.Context;

namespace Financas.Presentation.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterPresentationServices(this IServiceCollection services)
        {
            services.AddDbContext<FinanceiroDBContext>();

            services.RegisterServices();

            // EffectiveDate
            services.AddTransient<Modules.EffectiveDate.View.EffectiveDatePage>();
            services.AddTransient<Modules.EffectiveDate.ViewModels.EffectiveDateViewModel>();
            services.AddTransient<Modules.EffectiveDate.View.EffectiveDateDetailsPage>();
            services.AddTransient<Modules.EffectiveDate.ViewModels.EffectiveDateDetailsViewModel>();

            // Receivable
            services.AddTransient<Modules.Receivable.View.ReceivablePage>();
            services.AddTransient<Modules.Receivable.ViewModels.ReceivableViewModel>();
            services.AddTransient<Modules.Receivable.View.ReceivableFormPage>();
            services.AddTransient<Modules.Receivable.ViewModels.ReceivableFormViewModel>();

            // ImmediateExpense
            services.AddTransient<Modules.Payable.View.ImmediateExpensePage>();
            services.AddTransient<Modules.Payable.ViewModels.ImmediateExpenseViewModel>();
            services.AddTransient<Modules.Payable.View.ImmediateExpenseFormPage>();
            services.AddTransient<Modules.Payable.ViewModels.ImmediateExpenseFormViewModel>();

            // CardExpense
            services.AddTransient<Modules.Payable.View.CardExpenseFormPage>();
            services.AddTransient<Modules.Payable.ViewModels.CardExpenseFormViewModel>();
            services.AddTransient<Modules.Payable.View.CardExpenseItemFormPage>();
            services.AddTransient<Modules.Payable.ViewModels.CardExpenseItemFormViewModel>();

            return services;
        }
    }
}
