using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Financas.Application.Services.CompetenciaServices; 
using Financeiro.Application.Models.EffectiveDateModels; 
using System.Collections.ObjectModel;

namespace Financas.Presentation.Modules.EffectiveDate.ViewModels
{
    public partial class EffectiveDateViewModel : ObservableObject
    {
        private readonly IEffectiveDateServices _effectiveDateServices;

        [ObservableProperty]
        private ObservableCollection<EffectiveDateSummary> _items = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddEffectiveDateCommand))]
        private bool _isBusy;

        private EffectiveDateRequest _requestData;

        public EffectiveDateViewModel(IEffectiveDateServices effectiveDateServices)
        {
            _effectiveDateServices = effectiveDateServices;

            InitializeRequestData();
        }

        private void InitializeRequestData()
        {
            var now = DateTime.Now;
            _requestData = new EffectiveDateRequest
            {
                Month = now.ToString("MMM"),
                Year = now.Year
            };
        }

        [RelayCommand(CanExecute = nameof(CanExecuteAdd))]
        private async Task AddEffectiveDateAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                var result = await _effectiveDateServices.AddAsync(_requestData);

                if (result.IsSuccess)
                {
                    await Shell.Current.DisplayAlert("Sucesso", "Competência registrada!", "OK");
                    await GetDataAsync();
                }
                else
                {
                    var errorMessage = result.Error?.Message ?? "Erro desconhecido ao adicionar.";
                    await Shell.Current.DisplayAlert("Erro", errorMessage, "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Exceção", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanExecuteAdd() => !IsBusy;

        public async Task GetDataAsync()
        {
            IsBusy = true;
            try
            {
                var (result, lista) = await _effectiveDateServices.GetListAsync();

                if (result != null && result.IsSuccess && lista != null)
                {
                    Items.Clear();
                    foreach (var item in lista.OrderByDescending(x => x.Year).ThenByDescending(x => x.Month))
                    {
                        Items.Add(item);
                    }
                }
                else if (result != null)
                {
                    var errorMessage = result.Map(_ => (string?)null, err => err?.Message ?? "Erro desconhecido") ?? "Erro desconhecido";
                    await Shell.Current.DisplayAlert("Erro ao carregar", errorMessage, "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Exceção de Carregamento", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
        [RelayCommand]
        private async Task NavigateAsync(Guid effectiveDateId)
        {
            await Shell.Current.GoToAsync($"EffectiveDateDetailsPage?effectiveDateId={effectiveDateId.ToString()}");
        }

        [RelayCommand]
        private async Task DeleteAsync(Guid effectiveDateId)
        {
            bool confirm = await Shell.Current.DisplayAlert("Confirmar", "Deseja excluir esta competência?", "Sim", "Não");
            if (!confirm) return;

            IsBusy = true;
            try
            {
                var result = await _effectiveDateServices.DeleteAsync(effectiveDateId);
                if (result.IsSuccess)
                {
                    await Shell.Current.DisplayAlert("Sucesso", "Competência excluída!", "OK");
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