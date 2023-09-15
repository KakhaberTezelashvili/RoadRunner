using Microsoft.AspNetCore.Components.Routing;
using TDOC.Common.Client.Navigation.Models;

namespace TDOC.Common.Client.Navigation;

/// <summary>
/// NavigationManager wrapper containing <see cref="LocationChangingEventArgs" /> implementation.
/// </summary>
public class NavigationManagerWrapper : NavigationManager
{
    public event EventHandler<LocationChangingEventArgs> BeforeLocationChanged;

    private readonly NavigationManager _navigationManager;
    private bool ignoreNavigation;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationManagerWrapper" /> class.
    /// </summary>
    /// <param name="navigationManager">Instance of a NavigationManager that would be wrapped.</param>
    public NavigationManagerWrapper(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
        base.Initialize(navigationManager!.BaseUri, navigationManager.Uri);
        navigationManager.LocationChanged += OnLocationChanging;
    }

    /// <inheritdoc />
    protected override void EnsureInitialized() => base.Initialize(_navigationManager.BaseUri, _navigationManager.Uri);

    /// <inheritdoc />
    protected override void NavigateToCore(string uri, bool forceLoad) => _navigationManager.NavigateTo(uri, forceLoad);

    /// <summary>
    /// Intercepts LocationChanged event and triggers BeforeLocationChanged event,
    /// in case the navigation needs to be canceled.
    /// </summary>
    /// <param name="sender">Sender of location changed event.</param>
    /// <param name="e">Location changed event.</param>
    private void OnLocationChanging(object? sender, LocationChangedEventArgs e)
    {
        if (ignoreNavigation)
        {
            ignoreNavigation = false;
            return;
        }

        var navigation = TriggerBeforeLocationChanged(e);

        if (navigation.NavigationCanceled)
        {
            ignoreNavigation = true;
            _navigationManager.NavigateTo(Uri, false);
            return;
        }

        Uri = e.Location;
        // Trigger the Location Changed event for all listeners including the Router.
        NotifyLocationChanged(e.IsNavigationIntercepted);
        ignoreNavigation = false;
    }

    /// <summary>
    /// Triggers before location changed event.
    /// </summary>
    /// <param name="e">Location changed event, containing the new uri the user is navigating to.</param>
    private LocationChangingEventArgs TriggerBeforeLocationChanged(LocationChangedEventArgs e)
    {
        var eventArgs = new LocationChangingEventArgs()
        {
            NewLocation = e.Location
        };
        BeforeLocationChanged?.Invoke(this, eventArgs);
        return eventArgs;
    }
}