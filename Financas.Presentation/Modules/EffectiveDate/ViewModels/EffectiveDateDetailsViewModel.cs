using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Financas.Application.Services.CompetenciaServices;
using Financas.Presentation.Models;
using Financeiro.Application.Models.EffectiveDateModels;
using Financeiro.Application.Models.PayableModels;
using System.Collections.ObjectModel;

namespace Financas.Presentation.Modules.EffectiveDate.ViewModels
{
    public partial class EffectiveDateDetailsViewModel : ObservableObject
    {
        private readonly IEffectiveDateServices _effectiveDateServices;

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

        [ObservableProperty]
        private decimal _totalSubTotal;

        [ObservableProperty]
        private decimal _totalDiscounts;

        [ObservableProperty]
        private decimal _totalNet;

        public EffectiveDateDetailsViewModel(IEffectiveDateServices effectiveDateServices)
        {
            _effectiveDateServices = effectiveDateServices;
        }

        public async Task GetDataAsync(Guid id)
        {
            EffectiveDateId = id;
            IsBusy = true;
            try
            {
                var result = await _effectiveDateServices.GetByIdAsync(id);

                if (result.result.IsSuccess)
                {
                    View = result.view;
                    LoadTransactions();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro", result.result.Error?.Message ?? "Erro ao carregar", "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void LoadTransactions()
        {
            Transactions.Clear();

            foreach (var item in View.Receivable)
            {
                Transactions.Add(new TransactionItem
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
                Transactions.Add(new TransactionItem
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
                Transactions.Add(new TransactionItem
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

            var ordered = Transactions.OrderBy(t => t.Date).ToList();
            Transactions.Clear();
            foreach (var item in ordered)
            {
                Transactions.Add(item);
            }

            CalculateTotals();
        }

        private void CalculateTotals()
        {
            TotalSubTotal = Transactions.Sum(t => t.Type == TransactionType.Receivable ? t.SubTotal : -t.SubTotal);
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
