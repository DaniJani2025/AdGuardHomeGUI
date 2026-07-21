using AdGuardHomeGUI.Interfaces;
using AdGuardHomeGUI.Services;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AdGuardHomeGUI.Views;

public partial class MainWindow : Window
{
    private readonly AuthenticationService _authenticationService;
    private readonly AdGuardApiService _api;
    private readonly AdGuardService _service = new();
    private readonly DispatcherTimer _timer = new();
    private readonly BrowserService _browser = new();
    public bool ExitRequested { get; set; }
    public MainWindow(AuthenticationService authenticationService)
    {
        InitializeComponent();

        _authenticationService = authenticationService;
        _api = new AdGuardApiService(_authenticationService);

        Closing += MainWindow_Closing;

        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += (_, _) => UpdateServiceStatus();
        _timer.Start();

        UpdateServiceStatus();

        _ = UpdateProtectionStatus();
    }

    private async Task UpdateProtectionStatus()
    {
        bool enabled = await _api.GetProtectionStatusAsync();

        ProtectionStatus.Text = $"Protection: {(enabled ? "Enabled" : "Disabled")}";

        EnableProtectionButton.IsEnabled = !enabled;
        DisableProtectionButton.IsEnabled = enabled;
    }

    private void MainWindow_Closing(object? sender, CancelEventArgs e)
    {
        if (ExitRequested)
            return;
        
        e.Cancel = true;

        Hide();
        ShowInTaskbar = false;
    }

    private void UpdateServiceStatus()
    {
        var status = _service.GetStatus();

        ServiceStatus.Text = $"Service: {status}";

        switch (status)
        {
            case ServiceControllerStatus.Running:

                ServiceIndicator.Fill = Brushes.LimeGreen;

                StartServiceButton.IsEnabled = false;
                StopServiceButton.IsEnabled = true;

                break;

            case ServiceControllerStatus.Stopped:

                ServiceIndicator.Fill = Brushes.Red;

                StartServiceButton.IsEnabled = true;
                StopServiceButton.IsEnabled = false;

                break;

            case ServiceControllerStatus.StartPending:

                ServiceIndicator.Fill = Brushes.Gold;

                StartServiceButton.IsEnabled = false;
                StopServiceButton.IsEnabled = false;

                break;

            case ServiceControllerStatus.StopPending:

                ServiceIndicator.Fill = Brushes.Orange;

                StartServiceButton.IsEnabled = false;
                StopServiceButton.IsEnabled = false;

                break;

            default:

                ServiceIndicator.Fill = Brushes.Gray;

                StartServiceButton.IsEnabled = false;
                StopServiceButton.IsEnabled = false;

                break;
        }
    }

    private async void StartService_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            SetBusy(true);

            await Task.Run(() => _service.Start());

            UpdateServiceStatus();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error");
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async void StopService_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            SetBusy(true);

            await Task.Run(() => _service.Stop());

            UpdateServiceStatus();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error");
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async void EnableProtection_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            SetBusy(true);

            await _api.SetProtectionAsync(true);

            await UpdateProtectionStatus();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error");
        }
        finally
        {
            SetBusy(false);
            await UpdateProtectionStatus();
        }
    }

    private async void DisableProtection_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            SetBusy(true);

            await _api.SetProtectionAsync(false);

            await UpdateProtectionStatus();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error");
        }
        finally
        {
            SetBusy(false);
            await UpdateProtectionStatus();
        }
    }

    private void OpenDashboard_Click(object sender, RoutedEventArgs e)
    {
        _browser.OpenDashboard();
    }

    private void SetBusy(bool busy)
    {
        StartServiceButton.IsEnabled = !busy;
        StopServiceButton.IsEnabled = !busy;

        EnableProtectionButton.IsEnabled = !busy;
        DisableProtectionButton.IsEnabled = !busy;

        Mouse.OverrideCursor = busy
            ? Cursors.Wait
            : null;
    }
}