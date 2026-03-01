using RoyalBakeryGrn.Services;

namespace RoyalBakeryGrn.Pages;

public partial class LoginPage : ContentPage
{
    private readonly ApiClient _api;

    public LoginPage(ApiClient api)
    {
        InitializeComponent();
        _api = api;
    }

    private async void Login_Clicked(object sender, EventArgs e)
    {
        string username = (UsernameEntry.Text ?? "").Trim();
        string password = (PasswordEntry.Text ?? "").Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ShowError("Please enter username and password.");
            return;
        }

        if (!_api.IsConfigured)
        {
            ShowError("API URL not configured. Tap 'API Settings' below.");
            return;
        }

        LoginBtn.IsEnabled = false;
        LoginBtn.Text = "Logging in...";
        ErrorLabel.IsVisible = false;

        try
        {
            var result = await _api.LoginAsync(username, password);

            // Check role
            if (result.Role != "GRN" && result.Role != "Admin")
            {
                ShowError("This account does not have GRN access.");
                return;
            }

            // Store login info
            Preferences.Set("logged_in_user", result.DisplayName);
            Preferences.Set("logged_in_user_id", result.UserId);

            // Navigate to main page
            Application.Current!.MainPage = new NavigationPage(new MainPage(_api))
            {
                BarBackgroundColor = Color.FromArgb("#512BD4"),
                BarTextColor = Colors.White
            };
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
        finally
        {
            LoginBtn.IsEnabled = true;
            LoginBtn.Text = "Login";
        }
    }

    private async void Settings_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new SettingsPage(_api));
    }

    private void ShowError(string message)
    {
        ErrorLabel.Text = message;
        ErrorLabel.IsVisible = true;
    }
}
