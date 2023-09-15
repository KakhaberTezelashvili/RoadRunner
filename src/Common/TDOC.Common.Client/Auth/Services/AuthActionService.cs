using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using TDOC.Common.Client.Auth.Constants;
using TDOC.Common.Client.Http;
using TDOC.Common.Client.Utilities;
using TDOC.Common.Data.Auth.Constants;

namespace TDOC.Common.Client.Auth.Services;

/// <inheritdoc cref="IAuthActionService" />
public class AuthActionService : IAuthActionService
{
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly ILocalStorageService _localStorage;
    private readonly ITypedHttpClientFactory _httpClientFactory;
    private readonly NavigationManager _navigation;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthActionService" /> class.
    /// </summary>
    /// <param name="localStorage">Local storage.</param>
    /// <param name="httpClientFactory">Http client factory.</param>
    /// <param name="navigation"><see cref="NavigationManager"/></param>
    public AuthActionService(
        AuthenticationStateProvider authStateProvider,
        ILocalStorageService localStorage,
        ITypedHttpClientFactory httpClientFactory,
        NavigationManager navigation)
    {
        _authStateProvider = authStateProvider;
        _localStorage = localStorage;
        _httpClientFactory = httpClientFactory;
        _navigation = navigation;
    }

    /// <inheritdoc />
    public async Task ForceUserToBeLoginAsync(string loginUrl)
    {
        AuthenticationState authState = await _authStateProvider.GetAuthenticationStateAsync();
        if (!authState.User.Identity.IsAuthenticated
            && NavigationUtilities.GetUriSegmentIndex(_navigation, loginUrl, out _) == -1)
            _navigation.NavigateTo($"{loginUrl}?{AuthActionParameters.ReturnUrlQueryParameter}={Uri.EscapeDataString(_navigation.Uri)}");
    }

    /// <inheritdoc />
    public async Task LoginUserAsync(string authToken, string defaultUrl, string userName)
    {
        await _localStorage.SetItemAsync(AuthActionParameters.AuthTokenLocalStorageKey, authToken);
        _httpClientFactory.AuthToken = authToken;

        if (_authStateProvider is AuthStateProvider authStateProvider)
            await authStateProvider.NotifyAuthStateChangedAsync();

        string lastUser = await _localStorage.GetItemAsStringAsync(AuthActionParameters.LastUser);
        Uri uri = _navigation.ToAbsoluteUri(_navigation.Uri);
        Dictionary<string, StringValues> queryStrings = QueryHelpers.ParseQuery(uri.Query);
        if (userName.ToUpper() == lastUser && queryStrings.TryGetValue(AuthActionParameters.ReturnUrlQueryParameter, out StringValues returnUrl))
            _navigation.NavigateTo(returnUrl);
        else
            _navigation.NavigateTo(defaultUrl);
    }

    /// <inheritdoc />
    public async Task LogoutUserAsync(string loginUrl)
    {
        await _localStorage.SetItemAsStringAsync(AuthActionParameters.LastUser,
            (await _authStateProvider.GetAuthenticationStateAsync()).User.FindFirst(UserClaimTypes.Initials).Value.ToUpper());
        await _localStorage.RemoveItemAsync(AuthActionParameters.AuthTokenLocalStorageKey);
        await ForceUserToBeLoginAsync(loginUrl);
    }
}