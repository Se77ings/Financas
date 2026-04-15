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

        // Agrupamento por ano para exibição em linha com meses
        public ObservableCollection<YearGroup> ItemsGrouped { get; } = new();

        public class YearGroup
        {
            public int Year { get; set; }
            public ObservableCollection<EffectiveDateSummary> Months { get; set; } = new();
            public decimal Balance => Months?.Sum(m => m.Balance) ?? 0;
        }

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
                // antes de carregar, garantir que exista a competência do mês atual
                var now = DateTime.Now;
                var request = new EffectiveDateRequest { Month = now.ToString("MMM"), Year = now.Year };
                try
                {
                    await _effectiveDateServices.EnsureExistsAsync(request);
                }
                catch
                {
                    // ignorar falhas nessa etapa para não impedir a listagem
                }

                var (result, lista) = await _effectiveDateServices.GetListAsync();

                if (result != null && result.IsSuccess && lista != null)
                {
                    // construir agrupamento por ano: anos em ordem decrescente, meses em ordem decrescente
                    Items.Clear();
                    ItemsGrouped.Clear();

                    // ordenar lista por Year desc, Month desc (convertendo Month para número)
                    Func<string, int> monthToNumber = m =>
                    {
                        if (string.IsNullOrWhiteSpace(m)) return 0;
                        var mm = m.Trim().ToLowerInvariant();
                        return mm switch
                        {
                            "jan" => 1,
                            "fev" => 2,
                            "mar" => 3,
                            "abr" => 4,
                            "mai" => 5,
                            "jun" => 6,
                            "jul" => 7,
                            "ago" => 8,
                            "set" => 9,
                            "out" => 10,
                            "nov" => 11,
                            "dez" => 12,
                            _ =>
                                // tentar parse com cultura atual
                                (DateTime.TryParseExact(m, "MMM", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out var d) ? d.Month : 0)
                        };
                    };

                    var ordered = lista
                        .OrderByDescending(x => x.Year)
                        .ThenByDescending(x => monthToNumber(x.Month))
                        .ToList();

                    foreach (var item in ordered)
                    {
                        Items.Add(item);
                    }

                    // agrupar por ano
                    var groups = ordered.GroupBy(x => x.Year).OrderByDescending(g => g.Key);
                    foreach (var g in groups)
                    {
                        var group = new YearGroup { Year = g.Key };
                        foreach (var m in g.OrderByDescending(x => monthToNumber(x.Month)))
                            group.Months.Add(m);

                        ItemsGrouped.Add(group);
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