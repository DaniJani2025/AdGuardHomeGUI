using AdGuardHomeGUI.Interfaces;
using AdGuardHomeGUI.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace AdGuardHomeGUI.Services;

public class AdGuardApiService : IAdGuardApiService
{
    private readonly IAuthenticationService _authenticationService;

    public AdGuardApiService(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<bool> GetProtectionStatusAsync()
    {
        var response = await _authenticationService.HttpClient.GetAsync("/control/status");

        response.EnsureSuccessStatusCode();

        var status = await response.Content.ReadFromJsonAsync<StatusResponse>();

        return status!.ProtectionEnabled;
    }

        public async Task SetProtectionAsync(bool enabled)
        {
            var requestBody = new ProtectionRequest
            {
                Enabled = enabled,
                Duration = null
            };

            var json = JsonSerializer.Serialize(requestBody);

            using var request = new HttpRequestMessage(HttpMethod.Post, "/control/protection");

            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _authenticationService.HttpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }
}