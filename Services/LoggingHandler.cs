using System.Diagnostics;
using System.Net.Http;

namespace AdGuardHomeGUI.Services;

public class LoggingHandler : DelegatingHandler
{
    public LoggingHandler(HttpMessageHandler innerHandler)
        : base(innerHandler)
    {
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        Debug.WriteLine("========== REQUEST ==========");
        Debug.WriteLine($"{request.Method} {request.RequestUri}");

        foreach (var header in request.Headers)
            Debug.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");

        if (request.Content != null)
        {
            foreach (var header in request.Content.Headers)
                Debug.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");

            Debug.WriteLine("");
            Debug.WriteLine(await request.Content.ReadAsStringAsync());
        }

        Debug.WriteLine("=============================");

        var response = await base.SendAsync(request, cancellationToken);

        Debug.WriteLine("========== RESPONSE ==========");
        Debug.WriteLine($"{(int)response.StatusCode} {response.StatusCode}");

        foreach (var header in response.Headers)
            Debug.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");

        if (response.Content != null)
        {
            foreach (var header in response.Content.Headers)
                Debug.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");

            Debug.WriteLine("");
            Debug.WriteLine(await response.Content.ReadAsStringAsync());
        }

        Debug.WriteLine("==============================");

        return response;
    }
}