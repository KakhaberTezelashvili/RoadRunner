using Microsoft.JSInterop;
using System.Globalization;

namespace ScannerClient.WebApp.Shared
{
    public partial class CultureSelector
    {
        [Inject]
        private IJSRuntime _jsRuntime { get; set; }

        [Inject]
        private NavigationManager _navigation { get; set; }

        private readonly CultureInfo[] SupportedCultures = new[]
          {
            new CultureInfo("en-US"),
            new CultureInfo("fr-FR"),
          };

        private CultureInfo Culture
        {
            get => CultureInfo.CurrentCulture;
            set
            {
                if (CultureInfo.CurrentCulture != value)
                {
                    var js = (IJSInProcessRuntime)_jsRuntime;
                    js.InvokeVoid("appCulture.set", value.Name);

                    // TODO: Check after language selection is implemented.
                    _navigation.NavigateTo(_navigation.Uri);
                }
            }
        }
    }
}
