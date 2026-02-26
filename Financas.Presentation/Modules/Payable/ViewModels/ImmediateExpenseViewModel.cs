using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Financas.Application.Services.PayableServices.Interfaces;
using Financeiro.Application.Models.PayableModels;
using System.Collections.ObjectModel;

namespace Financas.Presentation.Modules.Payable.ViewModels
{
    public partial class ImmediateExpenseViewModel : ObservableObject
    {
        private readonly IImmediateExpenseServices _immediateExpenseServices;

        [ObservableProperty]
        private ObservableCollection<ImmediateExpenseView> _items = new();

        [ObservableProperty]
        private bool _isBusy;

        public ImmediateExpenseViewModel(IImmediateExpenseServices immediateExpenseServices)
        {
            _immediateExpenseServices = immediateExpenseServices;
        }

        public async Task GetDataAsync()
        {
            IsBusy = true;
            try
            {
                var (result, lista) = await _immediateExpenseServices.GetListAsync();

                if (result != null && result.IsSuccess && lista != null)
                {
                    Items.Clear();
                    foreach (var item in lista)
                    {
                        Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task NavigateToAddAsync()
        {
            await Shell.Current.GoToAsync("ImmediateExpenseFormPage");
        }

        [RelayCommand]
        private async Task NavigateToDetailsAsync(Guid id)
        {
            await Shell.Current.GoToAsync($"ImmediateExpenseFormPage?id={id.ToString()}");
        }

        [RelayCommand]
        private async Task DeleteAsync(Guid id)
        {
            bool confirm = await Shell.Current.DisplayAlert("Confirmar", "Deseja excluir esta despesa?", "Sim", "Não");
            if (!confirm) return;

            IsBusy = true;
            try
            {
                var result = await _immediateExpenseServices.DeleteAsync(id);
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
    }
}
