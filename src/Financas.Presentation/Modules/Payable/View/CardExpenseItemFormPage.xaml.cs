using Financas.Presentation.Modules.Payable.ViewModels;

namespace Financas.Presentation.Modules.Payable.View
{
    [QueryProperty(nameof(CardExpenseIdString), "cardExpenseId")]
    [QueryProperty(nameof(ItemIdString), "itemId")]
    public partial class CardExpenseItemFormPage : ContentPage
    {
        private readonly CardExpenseItemFormViewModel _viewModel;

        private string _cardExpenseIdString;
        public string CardExpenseIdString
        {
            get => _cardExpenseIdString;
            set
            {
                _cardExpenseIdString = value;
                if (Guid.TryParse(value, out var guid))
                {
                    _viewModel.CardExpenseId = guid;
                }
            }
        }

        private string _itemIdString;
        public string ItemIdString
        {
            get => _itemIdString;
            set
            {
                _itemIdString = value;
                if (Guid.TryParse(value, out var guid))
                {
                    _viewModel.ItemId = guid;
                }
            }
        }

        public CardExpenseItemFormPage(CardExpenseItemFormViewModel viewModel)
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
