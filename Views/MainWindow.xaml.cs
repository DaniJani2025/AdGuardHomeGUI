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

namespace AdGuardHomeGUI;

public partial class MainWindow : Window
{
    private readonly AdGuardService _service = new();
    private readonly DispatcherTimer _timer = new();
    private readonly BrowserService _browser = new();
    public bool ExitRequested { get; set; }
    public MainWindow()
    {
        InitializeComponent();
        TestLogin();
        Closing += MainWindow_Closing;

        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += (_, _) => UpdateServiceStatus();
        _timer.Start();

        UpdateServiceStatus();
    }

    private async void TestLogin()
    {
        var auth = new AuthenticationService();

        bool success = await auth.LoginAsync("dani", "Dcjjani@2000");

        if (!success)
        {
            MessageBox.Show("Login failed.");
            return;
        }

        var api = new AdGuardApiService(auth);

        await api.SetProtectionAsync(true);

        bool enabled = await api.GetProtectionStatusAsync();

        MessageBox.Show(enabled.ToString());
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
        
    }

    private async void DisableProtection_Click(object sender, RoutedEventArgs e)
    {

    }

    private void OpenDashboard_Click(object sender, RoutedEventArgs e)
    {
        _browser.OpenDashboard();
    }

    private void SetBusy(bool busy)
    {
        StartServiceButton.IsEnabled = !busy;
        StopServiceButton.IsEnabled = !busy;

        Mouse.OverrideCursor = busy
            ? Cursors.Wait
            : null;
    }
}