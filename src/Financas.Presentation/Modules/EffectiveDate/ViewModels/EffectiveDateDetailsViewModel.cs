using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Financas.Application.Services.EffectiveDateServices;
using Financas.Presentation.Models;
using Financeiro.Application.Models.EffectiveDateModels;
using Financeiro.Application.Models.PayableModels;
using Financeiro.Business.Aggregates.Payable;
using System.Collections.ObjectModel;

namespace Financas.Presentation.Modules.EffectiveDate.ViewModels
{
    public partial class EffectiveDateDetailsViewModel : ObservableObject
    {
        private readonly IEffectiveDateServices _effectiveDateServices;

        // Guarda todas as transações carregadas para permitir filtros locais
        private List<TransactionItem> _allTransactions = new();

        [ObservableProperty]
        private EffectiveDateView _view;

        [ObservableProperty]
        private Guid _effectiveDateId;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private ObservableCollection<TransactionItem> _transactions = new();

        [ObservableProperty]
        private TransactionItem _selectedTransaction;

        public bool HasSelectedTransaction => SelectedTransaction != null;

        partial void OnSelectedTransactionChanged(TransactionItem value)
        {
            OnPropertyChanged(nameof(HasSelectedTransaction));
        }

        private int _selectedFilterIndex = 0; // 0 = Todos, 1 = À Receber, 2 = À Pagar, 3 = Cartão

        public int SelectedFilterIndex
        {
            get => _selectedFilterIndex;
            set
            {
                if (SetProperty(ref _selectedFilterIndex, value))
                    ApplyFilter();
            }
        }

        [ObservableProperty]
        private decimal _totalPayable;
        [ObservableProperty]

        private decimal _totalReceivable;

        [ObservableProperty]
        private decimal _totalDiscounts;

        [ObservableProperty]
        private decimal _totalNet;

        public EffectiveDateDetailsViewModel(IEffectiveDateServices effectiveDateServices)
        {
            _effectiveDateServices = effectiveDateServices;
        }

        public async Task GetDataAsync(Guid id)
        { // o total e subtotal nao estao sendo calculados
            EffectiveDateId = id;
            IsBusy = true;
            try
            {
                var result = await _effectiveDateServices.GetByIdAsync(id);

                if (result.IsSuccess)
                {
                    View = result.Value;
                    LoadTransactions();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro", result.Error?.Message ?? "Erro ao carregar", "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void LoadTransactions()
        {
            // Evitar duplicação: construir lista temporária e então aplicar
            var all = new List<TransactionItem>();

            foreach (var item in View.Receivable)
            {
                all.Add(new TransactionItem
                {
                    Id = item.Id,
                    Description = item.Description,
                    Date = item.Date.ToDateTime(TimeOnly.MinValue),
                    SubTotal = item.SubTotal,
                    Discounts = item.Discounts,
                    Type = TransactionType.Receivable
                });
            }

            foreach (var item in View.Payable)
            {
                all.Add(new TransactionItem
                {
                    Id = item.Id,
                    Description = item.Description,
                    Date = item.DueDate.ToDateTime(TimeOnly.MinValue),
                    SubTotal = item.SubTotal,
                    Discounts = item.Discounts,
                    Type = TransactionType.ImmediateExpense
                });
            }

            foreach (var item in View.CardExpense)
            {
                all.Add(new TransactionItem
                {
                    Id = item.Id,
                    Description = item.Description,
                    Date = item.DueDate.ToDateTime(TimeOnly.MinValue),
                    SubTotal = item.SubTotal,
                    Discounts = item.Discounts,
                    Type = TransactionType.CardExpense,
                    IsCardExpense = true,
                    CardItems = item.Items
                });
            }

            // armazenar cópia completa e aplicar filtro
            _allTransactions = all;
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            System.Diagnostics.Debug.WriteLine($"ApplyFilter called. SelectedFilterIndex={SelectedFilterIndex}, _allTransactionsCount={_allTransactions?.Count}");
            Transactions.Clear();

            IEnumerable<TransactionItem> items = _allTransactions;

            switch (SelectedFilterIndex)
            {
                case 1: // À Receber
                    items = _allTransactions.Where(t => t.Type == TransactionType.Receivable);
                    break;
                case 2: // À Pagar (ImmediateExpense + CardExpense)
                    items = _allTransactions.Where(t => t.Type == TransactionType.ImmediateExpense || t.Type == TransactionType.CardExpense);
                    break;
                case 3: // Cartão
                    items = _allTransactions.Where(t => t.Type == TransactionType.CardExpense);
                    break;
                default:
                    items = _allTransactions;
                    break;
            }

            var ordered = items.OrderByDescending(t => t.Date).ToList();
            foreach (var item in ordered)
                Transactions.Add(item);

            CalculateTotals();
        }

        private void CalculateTotals()
        {

            decimal totalReceivable = 0;
            decimal totalPayable = 0;

            foreach (var item in Transactions)
            {

                if (item.Type == TransactionType.Receivable)
                {
                    totalReceivable += item.SubTotal;
                }
                else if (item.Type == TransactionType.ImmediateExpense || item.Type == TransactionType.CardExpense)
                {
                    totalPayable += item.SubTotal;
                }
            }

            TotalReceivable = totalReceivable;
            TotalPayable = totalPayable;
            TotalDiscounts = Transactions.Sum(t => t.Discounts);
            TotalNet = Transactions.Sum(t => t.Type == TransactionType.Receivable ? t.Total : -t.Total);
        }

        [RelayCommand]
        private async Task AddReceivableAsync()
        {
            await Shell.Current.GoToAsync($"ReceivableFormPage?effectiveDateId={EffectiveDateId}");
        }

        [RelayCommand]
        private async Task AddImmediateExpenseAsync()
        {
            await Shell.Current.GoToAsync($"ImmediateExpenseFormPage?effectiveDateId={EffectiveDateId}");
        }

        [RelayCommand]
        private async Task AddCardExpenseAsync()
        {
            await Shell.Current.GoToAsync($"CardExpenseFormPage?effectiveDateId={EffectiveDateId}");
        }

        [RelayCommand]
        private async Task EditTransactionAsync(TransactionItem transaction)
        {
            var route = transaction.Type switch
            {
                TransactionType.Receivable => "ReceivableFormPage",
                TransactionType.ImmediateExpense => "ImmediateExpenseFormPage",
                TransactionType.CardExpense => "CardExpenseFormPage",
                _ => "ReceivableFormPage"
            };
            await Shell.Current.GoToAsync($"{route}?id={transaction.Id}&effectiveDateId={EffectiveDateId}");
        }

        [RelayCommand]
        private void SelectTransaction(TransactionItem transaction)
        {
            if (transaction?.IsCardExpense == true)
            {
                SelectedTransaction = transaction;
            }
        }
    }
}
