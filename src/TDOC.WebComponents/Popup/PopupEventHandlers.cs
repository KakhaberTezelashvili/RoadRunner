namespace TDOC.WebComponents.Popup
{
    [EventHandler("ondxpopupclosing", typeof(EventArgs), true, true)]
    [EventHandler("ondxpopupclosed", typeof(EventArgs), true, true)]
    [EventHandler("ondxpopupshown", typeof(EventArgs), true, true)]
    [EventHandler("ondxpopupresize", typeof(EventArgs), true, true)]
    [EventHandler("ondxtitlechanged", typeof(ChangeEventArgs), true, true)]
    [EventHandler("ondxwidthchanged", typeof(ChangeEventArgs), true, true)]
    [EventHandler("ondxheightchanged", typeof(ChangeEventArgs), true, true)]
    public static class EventHandlers
    {
    }

    [BindElement("td-dx-popup", "Title", "data-dx-title", "ondxtitlechanged")]
    [BindElement("td-dx-popup", "Width", "data-dx-width", "ondxwidthchanged")]
    [BindElement("td-dx-popup", "Height", "data-dx-height", "ondxheightchanged")]
    public static class BindAttributes
    {
    }
}