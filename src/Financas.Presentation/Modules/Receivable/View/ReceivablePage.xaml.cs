using Financas.Presentation.Modules.Receivable.ViewModels;

namespace Financas.Presentation.Modules.Receivable.View
{
    public partial class ReceivablePage : ContentPage
    {
        private readonly ReceivableViewModel _viewModel;

        public ReceivablePage(ReceivableViewModel viewModel)
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
