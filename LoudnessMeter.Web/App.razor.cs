using Avalonia.Web.Blazor;

namespace LoudnessMeter.Web
{
    public partial class App
    {
        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            WebAppBuilder.Configure<LoudnessMeter.App>()
                .SetupWithSingleViewLifetime();
        }
    }
}