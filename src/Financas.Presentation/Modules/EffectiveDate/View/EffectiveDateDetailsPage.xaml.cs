using Financas.Presentation.Modules.EffectiveDate.ViewModels;

namespace Financas.Presentation.Modules.EffectiveDate.View;

[QueryProperty(nameof(EffectiveDateIdString), "effectiveDateId")]
public partial class EffectiveDateDetailsPage : ContentPage
{
    private readonly EffectiveDateDetailsViewModel _viewModel;

    private string _effectiveDateIdString;
    public string EffectiveDateIdString
    {
        get => _effectiveDateIdString;
        set
        {
            _effectiveDateIdString = value;
            if (Guid.TryParse(value, out var guid))
            {
                EffectiveDateId = guid;
            }
        }
    }

    public Guid EffectiveDateId { get; set; }

    public EffectiveDateDetailsPage(EffectiveDateDetailsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
        // garantir que mudança do Picker seja propagada ao ViewModel (fallback caso binding não atualize)
        var picker = this.FindByName<Picker>("FilterPicker");
        if (picker != null)
            picker.SelectedIndexChanged += FilterPicker_SelectedIndexChanged;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.GetDataAsync(EffectiveDateId);
    }

    void FilterPicker_SelectedIndexChanged(object? sender, EventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"FilterPicker_SelectedIndexChanged fired. SelectedIndex={(sender as Picker)?.SelectedIndex}");
        if (BindingContext is EffectiveDateDetailsViewModel vm && sender is Picker p)
        {
            vm.SelectedFilterIndex = p.SelectedIndex;
        }
    }
}