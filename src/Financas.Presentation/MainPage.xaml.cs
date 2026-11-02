using Financas.Application.Services.EffectiveDateServices;

namespace Financas.Presentation
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        private readonly IEffectiveDateServices _effectiveServices;

        public MainPage(IEffectiveDateServices effectiveServices)
        {
            InitializeComponent();
            _effectiveServices = effectiveServices;
        }

        private async void OnCounterClicked(object? sender, EventArgs e)
        {
            var foo = await _effectiveServices.GetListAsync();
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }
}
