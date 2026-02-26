namespace Financas.Presentation
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Registrar rotas de navegação
            Routing.RegisterRoute("EffectiveDateDetailsPage", typeof(Modules.EffectiveDate.View.EffectiveDateDetailsPage));
            Routing.RegisterRoute("ReceivableFormPage", typeof(Modules.Receivable.View.ReceivableFormPage));
            Routing.RegisterRoute("ImmediateExpenseFormPage", typeof(Modules.Payable.View.ImmediateExpenseFormPage));
            Routing.RegisterRoute("CardExpenseFormPage", typeof(Modules.Payable.View.CardExpenseFormPage));
            Routing.RegisterRoute("CardExpenseItemFormPage", typeof(Modules.Payable.View.CardExpenseItemFormPage));
        }
    }
}
