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
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.GetDataAsync(EffectiveDateId);
    }
}