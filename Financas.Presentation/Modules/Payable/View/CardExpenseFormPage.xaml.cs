using Financas.Presentation.Modules.Payable.ViewModels;

namespace Financas.Presentation.Modules.Payable.View
{
    [QueryProperty(nameof(IdString), "id")]
    [QueryProperty(nameof(EffectiveDateIdString), "effectiveDateId")]
    public partial class CardExpenseFormPage : ContentPage
    {
        private readonly CardExpenseFormViewModel _viewModel;

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

        public CardExpenseFormPage(CardExpenseFormViewModel viewModel)
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
