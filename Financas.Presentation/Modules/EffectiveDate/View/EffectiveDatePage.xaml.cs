using Financas.Presentation.Modules.EffectiveDate.ViewModels;

namespace Financas.Presentation.Modules.EffectiveDate.View
{
    public partial class EffectiveDatePage : ContentPage
    {
        private readonly EffectiveDateViewModel _viewModel;

        public EffectiveDatePage(EffectiveDateViewModel viewModel)
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