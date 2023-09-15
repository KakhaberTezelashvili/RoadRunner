using Microsoft.JSInterop;
using TDOC.WebComponents.Notification.Enumerations;
using TDOC.WebComponents.Notification.Models;

namespace TDOC.WebComponents.Notification;

public partial class TdToast : IDisposable
{
    [Inject]
    private IJSRuntime _jsRuntime { get; set; }

    [Inject]
    private CustomTimer _timer { get; set; }

    [Inject]
    private IMediatorCarrier _carrier { get; set; }

    private NotificationDetails notificationDetails;

    protected override void OnInitialized()
    {
        _carrier.Subscribe<HideToastNotification>(HideToast);
        _carrier.Subscribe<ShowToastNotification>(ShowToast);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await _jsRuntime.InvokeVoidAsync("devExtremeToastHelper.initToast");
    }

    public void Dispose()
    {
        _carrier.Unsubscribe<HideToastNotification>(HideToast);
        _carrier.Unsubscribe<ShowToastNotification>(ShowToast);
    }

    private void ShowToast(ShowToastNotification notification)
    {
        notificationDetails = notification.Details;
        StateHasChanged();

        // We are giving "a moment" for dxTemplate to apply new "notificationDetails".
        _timer.ExecActionAfterSomeDelay(ShowToast);
    }

    private void HideToast(HideToastNotification notification) => _timer.ExecActionAfterSomeDelay(InvokeHideToast);

    private void InvokeHideToast() => _jsRuntime.InvokeVoidAsync("devExtremeToastHelper.hideToast");

    private void ShowToast() => 
        _jsRuntime.InvokeVoidAsync("devExtremeToastHelper.showToast", 
            NotificationStyleKey(notificationDetails.Style), notificationDetails.CloseAutomatically, notificationDetails.CloseOnOutsideClick);

    private static string NotificationStyleKey(NotificationStyle value) => Enum.GetName(typeof(NotificationStyle), value).ToLower();
}