using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Financas.Application.Services.CompetenciaServices;
using Financas.Application.Services.DireitoServices;
using Financeiro.Application.Models.EffectiveDateModels;
using Financeiro.Application.Models.ReceivableModels;
using System.Collections.ObjectModel;

namespace Financas.Presentation.Modules.Receivable.ViewModels
{
    public partial class ReceivableFormViewModel : ObservableObject
    {
        private readonly ReceivableServices _receivableServices;
        private readonly IEffectiveDateServices _effectiveDateServices;

        [ObservableProperty]
        private Guid? _id;

        [ObservableProperty]
        private string _description;

        [ObservableProperty]
        private decimal _subTotal;

        [ObservableProperty]
        private decimal _discounts;

        [ObservableProperty]
        private Guid _effectiveDateId;

        [ObservableProperty]
        private ObservableCollection<EffectiveDateSummary> _effectiveDates = new();

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _isEditMode;

        [ObservableProperty]
        private DateTime _date = DateTime.Now;

        public ReceivableFormViewModel(ReceivableServices receivableServices, IEffectiveDateServices effectiveDateServices)
        {
            _receivableServices = receivableServices;
            _effectiveDateServices = effectiveDateServices;
        }

        public async Task GetDataAsync()
        {
            if (Id.HasValue)
            {
                IsEditMode = true;
                await LoadReceivableAsync(Id.Value);
            }
        }

        private async Task LoadReceivableAsync(Guid id)
        {
            IsBusy = true;
            try
            {
                var (result, view) = await _receivableServices.GetByIdAsync(id);
                if (result.IsSuccess && view != null)
                {
                    Description = view.Description;
                    SubTotal = view.SubTotal;
                    Discounts = view.Discounts;
                    Date = view.Date.ToDateTime(TimeOnly.MinValue);
                    EffectiveDateId = view.EffectiveDateId;
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
                var request = new ReceivableRequest
                {
                    Description = Description,
                    SubTotal = SubTotal,
                    Discounts = Discounts,
                    Date = DateOnly.FromDateTime(Date),
                    EffectiveDateId = EffectiveDateId
                };

                var result = IsEditMode
                    ? await _receivableServices.UpdateAsync(Id.Value, request)
                    : await _receivableServices.AddAsync(request);

                if (result.IsSuccess)
                {
                    await Shell.Current.DisplayAlert("Sucesso", "Conta a receber salva!", "OK");
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
                var request = new ReceivableRequest
                {
                    Description = Description,
                    SubTotal = SubTotal,
                    Discounts = Discounts,
                    Date = DateOnly.FromDateTime(Date),
                    EffectiveDateId = EffectiveDateId
                };

                var result = await _receivableServices.AddAsync(request);

                if (result.IsSuccess)
                {
                    Description = string.Empty;
                    SubTotal = 0;
                    Discounts = 0;
                    Date = Date;
                    await Shell.Current.DisplayAlert("Sucesso", "Conta salva! Adicione outra.", "OK");
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
            Date = Date.AddDays(1);
        }

        [RelayCommand]
        private void SubtractDay()
        {
            Date = Date.AddDays(-1);
        }

        [RelayCommand]
        private async Task DeleteAsync()
        {
            if (!Id.HasValue) return;

            bool confirm = await Shell.Current.DisplayAlert("Confirmar", "Deseja excluir esta conta a receber?", "Sim", "Não");
            if (!confirm) return;

            IsBusy = true;
            try
            {
                var result = await _receivableServices.DeleteAsync(Id.Value);
                if (result.IsSuccess)
                {
                    await Shell.Current.DisplayAlert("Sucesso", "Conta excluída!", "OK");
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
