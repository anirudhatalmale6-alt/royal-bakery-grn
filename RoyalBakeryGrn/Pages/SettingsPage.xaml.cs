using RoyalBakeryGrn.Services;

namespace RoyalBakeryGrn.Pages
{
    public partial class SettingsPage : ContentPage
    {
        private readonly ApiClient _api;

        public SettingsPage(ApiClient api)
        {
            InitializeComponent();
            _api = api;

            var saved = Preferences.Get("api_base_url", "");
            if (!string.IsNullOrEmpty(saved))
                ApiUrlEntry.Text = saved;
        }

        private async void SaveAndTest_Clicked(object sender, EventArgs e)
        {
            var url = ApiUrlEntry.Text?.Trim();
            if (string.IsNullOrEmpty(url))
            {
                await DisplayAlert("Error", "Please enter a URL", "OK");
                return;
            }

            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                url = "http://" + url;

            StatusLabel.Text = "Testing connection...";
            StatusLabel.TextColor = Colors.Gray;
            StatusLabel.IsVisible = true;

            try
            {
                _api.BaseUrl = url;
                var items = await _api.GetMenuItemsAsync();

                Preferences.Set("api_base_url", url);
                StatusLabel.Text = $"Connected! Found {items.Count} menu items.";
                StatusLabel.TextColor = Colors.Green;

                await DisplayAlert("Success", "Connection established!", "OK");
            }
            catch (Exception ex)
            {
                StatusLabel.Text = $"Connection failed: {ex.Message}";
                StatusLabel.TextColor = Colors.Red;
            }
        }
    }
}
