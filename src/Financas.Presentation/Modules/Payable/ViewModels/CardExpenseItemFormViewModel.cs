using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Financas.Application.Services.PayableServices.Interfaces;
using Financeiro.Application.Models.PayableModels;

namespace Financas.Presentation.Modules.Payable.ViewModels
{
    public partial class CardExpenseItemFormViewModel : ObservableObject
    {
        private readonly ICardExpenseServices _cardExpenseServices;

        [ObservableProperty]
        private Guid _cardExpenseId;

        [ObservableProperty]
        private Guid? _itemId;

        [ObservableProperty]
        private string _description;

        [ObservableProperty]
        private decimal _subTotal;

        [ObservableProperty]
        private decimal _discounts;

        [ObservableProperty]
        private DateTime _dueDate = DateTime.Now;

        [ObservableProperty]
        private string _observation;

        [ObservableProperty]
        private int _currentInstallment = 1;

        [ObservableProperty]
        private int _quantityInstallments = 1;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _isEditMode;

        public bool IsAddMode => !IsEditMode;

        public CardExpenseItemFormViewModel(ICardExpenseServices cardExpenseServices)
        {
            _cardExpenseServices = cardExpenseServices;
        }

        public async Task GetDataAsync()
        {
            if (ItemId.HasValue)
            {
                IsEditMode = true;
                await LoadItemAsync();
            }
            OnPropertyChanged(nameof(IsAddMode));
        }

        private async Task LoadItemAsync()
        {
            IsBusy = true;
            try
            {
                var (result, view) = await _cardExpenseServices.GetByIdAsync(CardExpenseId);
                if (result.IsSuccess && view != null)
                {
                    var item = view.Items.FirstOrDefault(i => i.Id == ItemId.Value);
                    if (item != null)
                    {
                        Description = item.Description;
                        SubTotal = item.SubTotal;
                        Discounts = item.Discounts;
                        DueDate = item.DueDate.ToDateTime(TimeOnly.MinValue);
                        Observation = item.Observation;
                        CurrentInstallment = item.CurrentInstallment;
                        QuantityInstallments = item.QuantityInstallments;
                    }
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(Description))
            {
                await Shell.Current.DisplayAlert("Atenção", "Preencha a descrição", "OK");
                return;
            }

            IsBusy = true;
            try
            {
                var request = new CardExpenseItemRequest
                {
                    Description = Description,
                    SubTotal = SubTotal,
                    Discounts = Discounts,
                    DueDate = DateOnly.FromDateTime(DueDate),
                    Observation = Observation,
                    CurrentInstallment = CurrentInstallment,
                    QuantityInstallments = QuantityInstallments
                };

                var result = IsEditMode
                    ? await _cardExpenseServices.UpdateItemAsync(CardExpenseId, ItemId.Value, request)
                    : await _cardExpenseServices.AddItemAsync(CardExpenseId, request);

                if (result.IsSuccess)
                {
                    await Shell.Current.DisplayAlert("Sucesso", "Item salvo!", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro", result.Error?.Message ?? "Erro ao salvar", "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task SaveAndContinueAsync()
        {
            if (string.IsNullOrWhiteSpace(Description))
            {
                await Shell.Current.DisplayAlert("Atenção", "Preencha a descrição", "OK");
                return;
            }

            IsBusy = true;
            try
            {
                var request = new CardExpenseItemRequest
                {
                    Description = Description,
                    SubTotal = SubTotal,
                    Discounts = Discounts,
                    DueDate = DateOnly.FromDateTime(DueDate),
                    Observation = Observation,
                    CurrentInstallment = CurrentInstallment,
                    QuantityInstallments = QuantityInstallments
                };

                var result = await _cardExpenseServices.AddItemAsync(CardExpenseId, request);

                if (result.IsSuccess)
                {
                    Description = string.Empty;
                    SubTotal = 0;
                    Discounts = 0;
                    Observation = string.Empty;
                    CurrentInstallment = 1;
                    QuantityInstallments = 1;
                    await Shell.Current.DisplayAlert("Sucesso", "Item salvo! Adicione outro.", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro", result.Error?.Message ?? "Erro ao salvar", "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void AddDay()
        {
            DueDate = DueDate.AddDays(1);
        }

        [RelayCommand]
        private void SubtractDay()
        {
            DueDate = DueDate.AddDays(-1);
        }


        [RelayCommand]
        private async Task DeleteAsync()
        {
            if (!ItemId.HasValue) return;

            bool confirm = await Shell.Current.DisplayAlert("Confirmar", "Deseja excluir este item?", "Sim", "Não");
            if (!confirm) return;

            IsBusy = true;
            try
            {
                var result = await _cardExpenseServices.RemoveItemAsync(CardExpenseId, ItemId.Value);
                if (result.IsSuccess)
                {
                    await Shell.Current.DisplayAlert("Sucesso", "Item excluído!", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro", result.Error?.Message ?? "Erro ao excluir", "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
