using AdGuardHomeGUI.Services;
using System.Windows;
using System.Windows.Input;

namespace AdGuardHomeGUI.Views;

public partial class LoginWindow : Window
{
    private readonly AuthenticationService _authenticationService = new();

    public LoginWindow()
    {
        InitializeComponent();

        UsernameTextBox.Focus();
    }

    private async void Login_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            SetBusy(true);

            StatusTextBlock.Text = "Logging in...";

            bool success = await _authenticationService.LoginAsync(
                UsernameTextBox.Text,
                PasswordBox.Password,
                RememberMeCheckBox.IsChecked == true);

            if (!success)
            {
                StatusTextBlock.Text = "Login failed.";

                MessageBox.Show(
                    "Invalid username or password.",
                    "Authentication Failed");

                return;
            }

            StatusTextBlock.Text = "Login successful.";

            var mainWindow = new MainWindow(_authenticationService);

            Application.Current.MainWindow = mainWindow;

            mainWindow.Show();

            Close();
        }
        finally
        {
            SetBusy(false);
        }
    }

    private void SetBusy(bool busy)
    {
        UsernameTextBox.IsEnabled = !busy;
        PasswordBox.IsEnabled = !busy;
        RememberMeCheckBox.IsEnabled = !busy;
        LoginButton.IsEnabled = !busy;

        Mouse.OverrideCursor = busy
            ? Cursors.Wait
            : null;
    }
}