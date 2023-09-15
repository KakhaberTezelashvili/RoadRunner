namespace TDOC.Common.Client.Navigation.Models;

/// <summary>
/// <see cref="EventArgs" /> for <see cref="NavigationManagerWrapper.BeforeLocationChanged" />.
/// </summary>
public class LocationChangingEventArgs : EventArgs
{
    /// <summary>
    /// Flag for canceled navigation.
    /// </summary>
    public bool NavigationCanceled { get; set; }

    /// <summary>
    /// New location after navigation.
    /// </summary>
    public string NewLocation { get; set; } = string.Empty;
}