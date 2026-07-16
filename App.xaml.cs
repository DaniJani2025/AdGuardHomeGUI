using H.NotifyIcon;
using System.Configuration;
using System.Data;
using System.Windows;

namespace AdGuardHomeGUI;

public partial class App : Application
{
    private TaskbarIcon TrayIcon =>
        (TaskbarIcon)Resources["TrayIcon"];

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        TrayIcon.ForceCreate();
    }

    private void TrayIcon_TrayLeftMouseUp(object? sender, RoutedEventArgs e)
    {
        if (Current.MainWindow is null)
            return;

        Current.MainWindow.Show();
        Current.MainWindow.ShowInTaskbar = true;
        Current.MainWindow.WindowState = WindowState.Normal;
        Current.MainWindow.Activate();
    }

    private void OpenTray_Click(object sender, RoutedEventArgs e)
    {
        Current.MainWindow!.Show();
        Current.MainWindow!.ShowInTaskbar = true;
        Current.MainWindow!.WindowState = WindowState.Normal;
        Current.MainWindow!.Activate();
    }

    private void ExitTray_Click(object sender, RoutedEventArgs e)
    {
        if (Current.MainWindow is MainWindow window)
        {
            window.ExitRequested = true;
            window.Close();
        }

        TrayIcon.Dispose();
        Shutdown();
    }
}

