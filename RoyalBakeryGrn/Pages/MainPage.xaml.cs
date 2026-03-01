using RoyalBakeryGrn.Services;

namespace RoyalBakeryGrn.Pages
{
    public partial class MainPage : ContentPage
    {
        private readonly ApiClient _api;

        public MainPage(ApiClient api)
        {
            InitializeComponent();
            _api = api;

            // Update date/time
            Dispatcher.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                DateTimeLabel.Text = DateTime.Now.ToString("dddd, MMMM dd yyyy HH:mm:ss");
                return true;
            });
        }

        // Navigation methods
        private async void AddGRNCard_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddGRN(_api));
        }

        private async void AdjustGRNCard_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AdjustmentRequestsListPage(_api));
        }

        private async void StockManageCard_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ClearancePage(_api));
        }
    }
}
