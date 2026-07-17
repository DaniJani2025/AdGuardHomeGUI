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

    public HttpClient HttpClient => _httpClient;

    public bool IsAuthenticated { get; private set; }

    public AuthenticationService()
    {
        _cookieContainer = new CookieContainer();

        var handler = new HttpClientHandler
        {
            CookieContainer = _cookieContainer
        };

        _httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://127.0.0.1")
        };
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        var request = new LoginRequest
        {
            Name = username,
            Password = password
        };

        var response = await _httpClient.PostAsJsonAsync("/control/login", request);

        if (!response.IsSuccessStatusCode)
        {
            IsAuthenticated = false;
            return false;
        }

        IsAuthenticated = true;
        return true;
    }

    public void Logout()
    {
        IsAuthenticated = false;
    }
}