using AdGuardHomeGUI.Interfaces;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using AdGuardHomeGUI.Models;

namespace AdGuardHomeGUI.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly CookieContainer _cookieContainer;
    private readonly HttpClient _httpClient;
    private readonly SessionStorageService _sessionStorage = new();

    public HttpClient HttpClient => _httpClient;

    public bool IsAuthenticated { get; private set; }

    public AuthenticationService()
    {
        _cookieContainer = new CookieContainer();

        var httpHandler = new HttpClientHandler
        {
            CookieContainer = _cookieContainer
        };

        var loggingHandler = new LoggingHandler(httpHandler);

        _httpClient = new HttpClient(loggingHandler)
        {
            BaseAddress = new Uri("http://127.0.0.1")
        };
    }

    private Cookie? GetAuthenticationCookie()
    {
        var cookies = _cookieContainer.GetCookies(new Uri("http://127.0.0.1"));

        foreach (Cookie cookie in cookies)
        {
            return cookie;
        }

        return null;
    }

    public async Task<bool> LoginAsync(
        string username,
        string password,
        bool rememberMe)
    {
        var request = new LoginRequest
        {
            Name = username,
            Password = password
        };

        var response = await _httpClient.PostAsJsonAsync(
            "/control/login",
            request);

        if (!response.IsSuccessStatusCode)
        {
            IsAuthenticated = false;
            return false;
        }

        IsAuthenticated = true;

        if (rememberMe)
        {
            Cookie? cookie = GetAuthenticationCookie();

            if (cookie != null)
            {
                _sessionStorage.Save(new SessionCookie
                {
                    Name = cookie.Name,
                    Value = cookie.Value,
                    Domain = cookie.Domain,
                    Path = cookie.Path,
                    Expires = cookie.Expires == DateTime.MinValue
                        ? null
                        : cookie.Expires
                });
            }
        }
        else
        {
            _sessionStorage.Delete();
        }

        return true;
    }

    public async Task<bool> TryRestoreSessionAsync()
    {
        try
        {
            SessionCookie? savedCookie = _sessionStorage.Load();

            if (savedCookie == null)
                return false;

            var cookie = new Cookie(
                savedCookie.Name,
                savedCookie.Value,
                savedCookie.Path,
                savedCookie.Domain);

            _cookieContainer.Add(cookie);

            var response = await _httpClient.GetAsync("/control/status");

            if (!response.IsSuccessStatusCode)
            {
                _sessionStorage.Delete();
                IsAuthenticated = false;
                return false;
            }

            IsAuthenticated = true;
            return true;
        }
        catch
        {
            _sessionStorage.Delete();
            IsAuthenticated = false;
            return false;
        }
    }

    public void Logout()
    {
        IsAuthenticated = false;
    }
}