using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using TDOC.Common.Client.Auth.Constants;
using TDOC.Common.Client.Http;

namespace TDOC.Common.Client.Auth.Services;

public class AuthStateProvider : AuthenticationStateProvider
{
    private readonly ITypedHttpClientFactory _httpClientFactory;
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationState _anonymous;

    public AuthStateProvider(ITypedHttpClientFactory httpClientFactory, ILocalStorageService localStorage)
    {
        _httpClientFactory = httpClientFactory;
        _localStorage = localStorage;
        _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string authToken = await _localStorage.GetItemAsync<string>(AuthActionParameters.AuthTokenLocalStorageKey);
        if (string.IsNullOrWhiteSpace(authToken))
            return _anonymous;
        _httpClientFactory.AuthToken = authToken;
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(authToken), AuthActionParameters.JWTAuthType)));
    }

    public async Task NotifyAuthStateChangedAsync()
    {
        string authToken = await _localStorage.GetItemAsync<string>(AuthActionParameters.AuthTokenLocalStorageKey);
        if (authToken != null)
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(
                new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(authToken), AuthActionParameters.JWTAuthType)))));
        else
            NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
    }
}