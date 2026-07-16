using System.Diagnostics;

namespace AdGuardHomeGUI.Services;

public class BrowserService
{
    public void Open(string url)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }

    public void OpenDashboard() => Open("http://127.0.0.1");
}