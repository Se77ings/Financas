using Financas.Presentation.Modules.Receivable.ViewModels;

namespace Financas.Presentation.Modules.Receivable.View
{
    [QueryProperty(nameof(IdString), "id")]
    [QueryProperty(nameof(EffectiveDateIdString), "effectiveDateId")]
    public partial class ReceivableFormPage : ContentPage
    {
        private readonly ReceivableFormViewModel _viewModel;

        private string _idString;
        public string IdString
        {
            get => _idString;
            set
            {
                _idString = value;
                if (Guid.TryParse(value, out var guid))
                {
                    _viewModel.Id = guid;
                }
            }
        }

        private string _effectiveDateIdString;
        public string EffectiveDateIdString
        {
            get => _effectiveDateIdString;
            set
            {
                _effectiveDateIdString = value;
                if (Guid.TryParse(value, out var guid))
                {
                    _viewModel.EffectiveDateId = guid;
                }
            }
        }

        public ReceivableFormPage(ReceivableFormViewModel viewModel)
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
