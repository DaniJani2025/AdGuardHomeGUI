using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using AdGuardHomeGUI.Interfaces;
using AdGuardHomeGUI.Models;

namespace AdGuardHomeGUI.Services;

public class AdGuardApi : IAdGuardApiService
{
    private readonly HttpClient _client;

    public AdGuardApi()
    {
        var handler = new HttpClientHandler
        {
            CookieContainer = new CookieContainer()
        };

        _client = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://127.0.0.1")
        };
    }

    public async Task<bool> IsProtectionEnabled()
    {
        // We'll implement this next.
        throw new NotImplementedException();
    }

    public async Task SetProtection(bool enabled)
    {
        var response = await _client.PostAsJsonAsync(
            "/control/protection",
            new
            {
                enabled
            });

        response.EnsureSuccessStatusCode();
    }
}