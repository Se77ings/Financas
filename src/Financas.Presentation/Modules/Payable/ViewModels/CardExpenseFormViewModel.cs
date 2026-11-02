using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Financas.Application.Services.PayableServices.Interfaces;
using Financeiro.Application.Models.PayableModels;
using System.Collections.ObjectModel;

namespace Financas.Presentation.Modules.Payable.ViewModels
{
    public partial class CardExpenseFormViewModel : ObservableObject
    {
        private readonly ICardExpenseServices _cardExpenseServices;

        [ObservableProperty]
        private Guid? _id;

        [ObservableProperty]
        private string _description;

        [ObservableProperty]
        private string _observation;

        [ObservableProperty]
        private Guid _effectiveDateId;

        [ObservableProperty]
        private ObservableCollection<CardExpenseItemView> _items = new();

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _isEditMode;

        [ObservableProperty]
        private DateTime _dueDate = DateTime.Now;

        public CardExpenseFormViewModel(ICardExpenseServices cardExpenseServices)
        {
            _cardExpenseServices = cardExpenseServices;
        }

        public async Task GetDataAsync()
        {
            if (Id.HasValue)
            {
                IsEditMode = true;
                await LoadExpenseAsync(Id.Value);
            }
        }

        private async Task LoadExpenseAsync(Guid id)
        {
            IsBusy = true;
            try
            {
                var result = await _cardExpenseServices.GetByIdAsync(id);
                var view = result.Value;
                if (result.IsSuccess && view != null)
                {
                    Description = view.Description;
                    DueDate = view.DueDate.ToDateTime(TimeOnly.MinValue);
                    Observation = view.Observation;
                    Items.Clear();
                    foreach (var item in view.Items)
                    {
                        Items.Add(item);
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
            if (string.IsNullOrWhiteSpace(Description) || EffectiveDateId == Guid.Empty)
            {
                await Shell.Current.DisplayAlert("Atenção", "Preencha todos os campos obrigatórios", "OK");
                return;
            }

            IsBusy = true;
            try
            {
                var request = new CardExpenseRequest
                {
                    Description = Description,
                    SubTotal = 0,
                    Discounts = 0,
                    DueDate = DateOnly.FromDateTime(DueDate),
                    Observation = Observation,
                    EffectiveDate = EffectiveDateId
                };

                var result = IsEditMode
                    ? await _cardExpenseServices.UpdateAsync(Id.Value, request)
                    : await _cardExpenseServices.AddAsync(request);

                if (result.IsSuccess)
                {
                    await Shell.Current.DisplayAlert("Sucesso", "Fatura salva!", "OK");
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
            if (string.IsNullOrWhiteSpace(Description) || EffectiveDateId == Guid.Empty)
            {
                await Shell.Current.DisplayAlert("Atenção", "Preencha todos os campos obrigatórios", "OK");
                return;
            }

            IsBusy = true;
            try
            {
                var request = new CardExpenseRequest
                {
                    Description = Description,
                    SubTotal = 0,
                    Discounts = 0,
                    DueDate = DateOnly.FromDateTime(DueDate),
                    Observation = Observation,
                    EffectiveDate = EffectiveDateId
                };

                var result = await _cardExpenseServices.AddAsync(request);

                if (result.IsSuccess)
                {
                    Description = string.Empty;
                    Observation = string.Empty;
                    DueDate = DueDate;
                    await Shell.Current.DisplayAlert("Sucesso", "Fatura salva! Adicione outra.", "OK");
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
        private async Task AddItemAsync()
        {
            if (!Id.HasValue)
            {
                await Shell.Current.DisplayAlert("Atenção", "Salve a fatura antes de adicionar itens", "OK");
                return;
            }
            await Shell.Current.GoToAsync($"CardExpenseItemFormPage?cardExpenseId={Id.Value}");
        }

        [RelayCommand]
        private async Task DeleteAsync()
        {
            if (!Id.HasValue) return;

            bool confirm = await Shell.Current.DisplayAlert("Confirmar", "Deseja excluir esta fatura?", "Sim", "Não");
            if (!confirm) return;

            IsBusy = true;
            try
            {
                var result = await _cardExpenseServices.DeleteAsync(Id.Value);
                if (result.IsSuccess)
                {
                    await Shell.Current.DisplayAlert("Sucesso", "Fatura excluída!", "OK");
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

        [RelayCommand]
        private async Task DeleteItemAsync(Guid itemId)
        {
            if (!Id.HasValue) return;

            bool confirm = await Shell.Current.DisplayAlert("Confirmar", "Deseja excluir este item?", "Sim", "Não");
            if (!confirm) return;

            IsBusy = true;
            try
            {
                var result = await _cardExpenseServices.RemoveItemAsync(Id.Value, itemId);
                if (result.IsSuccess)
                {
                    await GetDataAsync();
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

        [RelayCommand]
        private async Task EditItemAsync(Guid itemId)
        {
            if (!Id.HasValue) return;
            await Shell.Current.GoToAsync($"CardExpenseItemFormPage?cardExpenseId={Id.Value}&itemId={itemId}");
        }

        [RelayCommand]
        private void CheckAll()
        {
            foreach (var item in Items)
                item.IsChecked = true;
        }

        [RelayCommand]
        private void UncheckAll()
        {
            foreach (var item in Items)
                item.IsChecked = false;
        }
    }
}
