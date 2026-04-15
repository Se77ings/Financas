using Financas.Presentation.Modules.Payable.ViewModels;

namespace Financas.Presentation.Modules.Payable.View
{
    public partial class ImmediateExpensePage : ContentPage
    {
        private readonly ImmediateExpenseViewModel _viewModel;

        public ImmediateExpensePage(ImmediateExpenseViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.GetDataAsync();
        }
    }
}
