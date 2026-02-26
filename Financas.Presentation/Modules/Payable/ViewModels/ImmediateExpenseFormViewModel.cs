using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Financas.Application.Services.CompetenciaServices;
using Financas.Application.Services.PayableServices.Interfaces;
using Financeiro.Application.Models.EffectiveDateModels;
using Financeiro.Application.Models.PayableModels;
using System.Collections.ObjectModel;

namespace Financas.Presentation.Modules.Payable.ViewModels
{
    public partial class ImmediateExpenseFormViewModel : ObservableObject
    {
        private readonly IImmediateExpenseServices _immediateExpenseServices;
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
        private DateTime _dueDate = DateTime.Now;

        [ObservableProperty]
        private string _observation;

        [ObservableProperty]
        private Guid _effectiveDateId;

        [ObservableProperty]
        private ObservableCollection<EffectiveDateSummary> _effectiveDates = new();

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _isEditMode;

        [ObservableProperty]
        private DateTime _lastSavedDate = DateTime.Now;

        public ImmediateExpenseFormViewModel(IImmediateExpenseServices immediateExpenseServices, IEffectiveDateServices effectiveDateServices)
        {
            _immediateExpenseServices = immediateExpenseServices;
            _effectiveDateServices = effectiveDateServices;
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
                var (result, view) = await _immediateExpenseServices.GetByIdAsync(id);
                if (result.IsSuccess && view != null)
                {
                    Description = view.Description;
                    SubTotal = view.SubTotal;
                    Discounts = view.Discounts;
                    DueDate = view.DueDate.ToDateTime(TimeOnly.MinValue);
                    Observation = view.Observation;
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
                var request = new ImmediateExpenseRequest
                {
                    Description = Description,
                    SubTotal = SubTotal,
                    Discounts = Discounts,
                    DueDate = DateOnly.FromDateTime(DueDate),
                    Observation = Observation,
                    EffectiveDateId = EffectiveDateId
                };

                var result = IsEditMode
                    ? await _immediateExpenseServices.UpdateAsync(Id.Value, request)
                    : await _immediateExpenseServices.AddAsync(request);

                if (result.IsSuccess)
                {
                    await Shell.Current.DisplayAlert("Sucesso", "Despesa salva!", "OK");
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
                var request = new ImmediateExpenseRequest
                {
                    Description = Description,
                    SubTotal = SubTotal,
                    Discounts = Discounts,
                    DueDate = DateOnly.FromDateTime(DueDate),
                    Observation = Observation,
                    EffectiveDateId = EffectiveDateId
                };

                var result = await _immediateExpenseServices.AddAsync(request);

                if (result.IsSuccess)
                {
                    LastSavedDate = DueDate;
                    Description = string.Empty;
                    SubTotal = 0;
                    Discounts = 0;
                    Observation = string.Empty;
                    await Shell.Current.DisplayAlert("Sucesso", "Despesa salva! Adicione outra.", "OK");
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
            if (!Id.HasValue) return;

            bool confirm = await Shell.Current.DisplayAlert("Confirmar", "Deseja excluir esta despesa?", "Sim", "Não");
            if (!confirm) return;

            IsBusy = true;
            try
            {
                var result = await _immediateExpenseServices.DeleteAsync(Id.Value);
                if (result.IsSuccess)
                {
                    await Shell.Current.DisplayAlert("Sucesso", "Despesa excluída!", "OK");
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
