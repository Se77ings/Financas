using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Financas.Application.Services.DireitoServices;
using Financeiro.Application.Models.ReceivableModels;
using System.Collections.ObjectModel;

namespace Financas.Presentation.Modules.Receivable.ViewModels
{
    public partial class ReceivableViewModel : ObservableObject
    {
        private readonly ReceivableServices _receivableServices;

        [ObservableProperty]
        private ObservableCollection<ReceivableSummary> _items = new();

        [ObservableProperty]
        private bool _isBusy;

        public ReceivableViewModel(ReceivableServices receivableServices)
        {
            _receivableServices = receivableServices;
        }

        public async Task GetDataAsync()
        {
            IsBusy = true;
            try
            {
                var (result, lista) = await _receivableServices.GetListAsync();

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
            await Shell.Current.GoToAsync("ReceivableFormPage");
        }

        [RelayCommand]
        private async Task NavigateToDetailsAsync(Guid id)
        {
            await Shell.Current.GoToAsync($"ReceivableFormPage?id={id.ToString()}");
        }

        [RelayCommand]
        private async Task DeleteAsync(Guid id)
        {
            bool confirm = await Shell.Current.DisplayAlert("Confirmar", "Deseja excluir esta conta a receber?", "Sim", "Não");
            if (!confirm) return;

            IsBusy = true;
            try
            {
                var result = await _receivableServices.DeleteAsync(id);
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
